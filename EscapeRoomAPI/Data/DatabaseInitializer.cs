using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Services;
using EscapeRoomAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EscapeRoomAPI.Data;

public static class DatabaseInitialiserExtension
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        // Create IServiceScope to resolve service scope
        using (var scope = app.Services.CreateScope())
        {
            var initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

            await initialiser.InitialiseAsync();

            // Try to seeding data
            await initialiser.SeedAsync();

            await Task.CompletedTask;
        }
    }
}

public interface IDatabaseInitializer
{
    Task InitialiseAsync();
    Task SeedAsync();
    Task TrySeedAsync();
}


public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly EscapeRoomUnityContext _context;
    private readonly FirebaseCredentials _fbCredentails;
    private readonly AppSettings _appSettings;
    private readonly IFirebaseService _firebaseService;

    public DatabaseInitializer(EscapeRoomUnityContext context,
        IOptionsMonitor<FirebaseCredentials> fbMonitor,
        IOptionsMonitor<AppSettings> asMonitor,
        IFirebaseService firebaseService)
    {
        _context = context;
        _fbCredentails = fbMonitor.CurrentValue;
        _appSettings = asMonitor.CurrentValue;
        _firebaseService = firebaseService;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // Check if database is not exist 
            if (!_context.Database.CanConnect())
            {
                // Migration Database - Create database 
                await _context.Database.MigrateAsync();
            }

            // Check if migrations have already been applied 
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            if (appliedMigrations.Any())
            {
                Console.WriteLine("Migrations have already been applied. Skip migratons proccess.");
                return;
            }

            Console.WriteLine("Database migrated successfully");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        Console.WriteLine("--> Seeding Data");
        try
        {
            // Firebase directory
            var firebaseDirectory = _appSettings.FirebaseDirectory;

            await Task.CompletedTask;            
        }
        catch (Exception)
        {
            throw;
        }
    }
}