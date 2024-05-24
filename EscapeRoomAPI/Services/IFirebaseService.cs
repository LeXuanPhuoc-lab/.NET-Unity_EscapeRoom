using System.IO.Compression;
using EscapeRoomAPI.Utils;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace EscapeRoomAPI.Services;

public interface IFirebaseService
{
    Task<string> UploadItemAsync(IFormFile file, string directoryPath, string fileName);
    Task<bool> UpdateItemFileNameAsync(string directoryPath, string oldFileName, string newFileName, IFormFile? newFile);
    Task<bool> DeleteItemAsync(string directoryPath, string fileName);
    Task<FileStream> RetrieveItemAsync(string rootPath);
    Task<FileStream> RetrieveItemZipAsync(Guid id, string localZipFilePath);
    Task<FileStream> RetrieveItemZipWithListPatternAsync(IEnumerable<string> pattern);
    bool IsExistDirectory(string directoryPath);
}

public class FirebaseService : IFirebaseService
{
    private readonly StorageClient _storageClient;
    private readonly FirebaseCredentials _fbCredentials;

    public FirebaseService(IOptionsMonitor<FirebaseCredentials> monitor)
    {
        _fbCredentials = monitor.CurrentValue;

        // Check Default Firebase Credentials exist or not 
        if (FirebaseApp.DefaultInstance == null)
        {
            // Get firebase configuration root path
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase.json");

            // Set EnvironmentVariable to configuraton file
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", rootPath);

            // Convert file stream to text
            var json = File.ReadAllText(rootPath);

            // Define google credential 
            var credential = GoogleCredential.FromJson(json);
            // Create default app instance with specified options
            FirebaseApp.Create(new AppOptions
            {
                ProjectId = _fbCredentials.ProjectId,
                Credential = credential
            });

        }
        // Default application credential
        _storageClient = StorageClient.Create();
    }

    public bool IsExistDirectory(string directoryPath)
    {
        // Get List object by DirectoryPath + Filename
        var objects = _storageClient.ListObjects(_fbCredentials?.BucketName, directoryPath);
        // Check exist first 
        var prevObj = objects.FirstOrDefault();
        // If not exist 
        if (prevObj is null) return false;

        return true;
    }

    // Summary:
    //      Upload item to firebase storage
    public async Task<string> UploadItemAsync(IFormFile file, string directoryPath, string fileName)
    {
        // Configuration firebase auth
        var config = new FirebaseAuthConfig
        {
            ApiKey = _fbCredentials.ApiKey, // Firebase API Key
            AuthDomain = $"{_fbCredentials.ProjectId}.firebaseapp.com", // Project ID domain
            Providers = new FirebaseAuthProvider[]{
                new EmailProvider() // Enable Email Auth Provider 
            }
        };

        // Handle auth 
        var client = new FirebaseAuthClient(config);
        // Sign in with email and password
        var auth = await client.SignInWithEmailAndPasswordAsync(
            _fbCredentials.AuthEmail,
            _fbCredentials.AuthPassword);

        // Cancellation token source
        var cancellation = new CancellationTokenSource();

        // Directory child elements
        var directoryChilds = DirectoryChildHelper.SplitSubDirectory(directoryPath);

        // Establish connect to FirebaseStorage by User Token Id and Bucket name
        var task = new FirebaseStorage(
            _fbCredentials.BucketName,
            new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = async () => await auth.User.GetIdTokenAsync(),
                ThrowOnCancel = true
            })
        .Child(directoryChilds[0]);

        // Generate directory
        foreach (var dc in directoryChilds.Skip(1))
        {
            task.Child(dc);
        }

        // Adding file
        task.Child($"{fileName}");

        // Convert to File Stream
        // Adding file to directory and push to firebase storage
        await task.PutAsync(file.OpenReadStream(), cancellation.Token);

        // Get the download URL of the uploaded file
        string downloadUrl = await task.GetDownloadUrlAsync();

        return downloadUrl;
    }

    //  Summary:
    //      Get single file from root path then return file stream
    public async Task<FileStream> RetrieveItemAsync(string rootPath)
    {
        try
        {
            // Create temporary file to save the memory stream contents
            var fileName = Path.GetTempFileName();

            // Create an empty zip file
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {

                using (var stream = new MemoryStream())
                {
                    // Download the file contents
                    await _storageClient.DownloadObjectAsync(_fbCredentials?.BucketName, rootPath, stream);

                    // Set the position of the memory stream to the beginning
                    stream.Seek(0, SeekOrigin.Begin);

                    // Copy the contents of the memory stream to the file stream
                    await stream.CopyToAsync(fileStream);
                }
            }

            // Return FileStream for the file
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured: {ex.Message}");
        }

        return null!;
    }

    // Summary:
    //      Get all files, subdirectories in particular root path
    public async Task<FileStream> RetrieveItemZipAsync(Guid id, string localZipFilePath)
    {
        try
        {
            // Create a zip file containing the directory
            await CreateZipFileAsync(id.ToString(), localZipFilePath);

            // Send the zip file in response
            var fileStream = new FileStream(localZipFilePath, FileMode.Open);
            return fileStream;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured: {ex.Message}");
        }

        return null!;
    }

    // Summary:
    //      Create zip file at particular directory path from firebase
    private async Task CreateZipFileAsync(string directoryPath, string zipFilePath)
    {
        // Create an empty zip file
        using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
        {
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                // Get the list of files in the directory from Firebase Storage
                var listObjectsOptions = new ListObjectsOptions
                {
                    // IncludeFoldersAsPrefixes = true
                };
                var objects = _storageClient.ListObjects(_fbCredentials?.BucketName, directoryPath);

                foreach (var obj in objects)
                {
                    using (var stream = new MemoryStream())
                    {
                        // Download the file contents
                        await _storageClient.DownloadObjectAsync(_fbCredentials?.BucketName, obj.Name, stream);

                        // Add the file to zip archive 
                        var entry = archive.CreateEntry(obj.Name, CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            stream.Position = 0;
                            stream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }
    }

    // Summary:
    //      Get all files, subdirectories in list of root path
    public async Task<FileStream> RetrieveItemZipWithListPatternAsync(
        IEnumerable<string> patterns)
    {
        try
        {
            // Create a temporary file to write the memory stream content 
            var zipFilePath = Path.GetTempFileName();

            // Create zip file with list of root directory
            await CreateZipFileWithListRootAsync(
                // Convert to array of patterns
                patterns.ToArray(),
                // local temp zip file path 
                zipFilePath
            );

            // Send the zip file in response
            var fileStream = new FileStream(zipFilePath, FileMode.Open);
            return fileStream;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occured: " + ex.Message);
        }

        return null!;
    }

    // Summary:
    //      Create zip file for all files, subdiretories in list of root path
    private async Task CreateZipFileWithListRootAsync(string[] directoryPaths, string zipFilePath)
    {
        // Create an empty zip file
        using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
        {
            // Initiate zip archive from particular stream
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                // Loop all path in root directory
                foreach (var path in directoryPaths)
                {
                    // Get list objects 
                    var objects = _storageClient.ListObjects(_fbCredentials?.BucketName, path);

                    // With each object in PageEnumerable<Storage.Object>
                    foreach (var obj in objects)
                    {
                        // Generate empty stream memory 
                        using (var stream = new MemoryStream())
                        {
                            // Download the file contents
                            await _storageClient.DownloadObjectAsync(_fbCredentials?.BucketName, obj.Name, stream);

                            // Add the file to zip archive 
                            var entry = archive.CreateEntry(obj.Name, CompressionLevel.Optimal);
                            using (var entryStream = entry.Open())
                            {
                                stream.Position = 0;
                                await stream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
            }
        }
    }

    // Summary:
    //      Update Item in firebase storage
    public async Task<bool> UpdateItemFileNameAsync(string directoryPath, string oldFileName, string newFileName, IFormFile? newFile)
    {
        // Check exist path directory
        if (!IsExistDirectory(directoryPath)) return false;

        // Get List object by DirectoryPath + Filename
        var objects = _storageClient.ListObjects(_fbCredentials?.BucketName, directoryPath);

        // Find the object with the old filename
        var prevObj = objects.FirstOrDefault(x => x.Name.EndsWith(oldFileName));

        if (prevObj != null)
        {
            // Generate the new name for the object
            string newObjectName = directoryPath + "/" + newFileName;

            if (newFile == null) // If not update file, just name of file
            {
                // Copy the object with the new name
                await _storageClient.CopyObjectAsync(
                    _fbCredentials?.BucketName, prevObj.Name,
                    _fbCredentials?.BucketName, newObjectName);

            }
            else
            {
                // Create a new instance of FormFile
                var customFile = new FormFile(newFile.OpenReadStream(), 0, newFile.Length, null!, newFileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = newFile.ContentType
                };

                await UploadItemAsync(customFile, directoryPath, customFile.FileName);
            }

            // Delete the old object
            await _storageClient.DeleteObjectAsync(_fbCredentials?.BucketName, prevObj.Name);

            return true;
        }

        return false;
    }
    
    // Summary:
    //      Delete item in firebase storage
    public async Task<bool> DeleteItemAsync(string directoryPath, string fileName)
    {
        try
        {
            // Combine file path
            var filePath = Path.Combine(directoryPath, fileName);
            // Replaces slashes type (if any)
            filePath = filePath.Replace(@"\\", "/").Replace(@"\", "/");


            // List objects (files) in bucket
            var objects = _storageClient.ListObjects(
                _fbCredentials?.BucketName, directoryPath);

            // Check exist bucket directory or not 
            if (objects.Any(obj => obj.Name == filePath))
            {
                // Delete item
                _storageClient.DeleteObject(_fbCredentials?.BucketName, filePath);
                Console.WriteLine($"File {fileName} deleted successfully from FireBase Storage");
                return true;
            }
            else
            {
                Console.WriteLine($"File {fileName} does not exist in FireBase Storage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An Error Occured: " + ex.Message);
        }
        finally
        {
            await Task.CompletedTask;
        }

        return false;
    }

}