using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Services;
using EscapeRoomAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EscapeRoomAPI.Data;

public static class DatabaseInitialiserExtension
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        // Create IServiceScope to resolve service scope
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

            await initializer.InitializeAsync();

            // Try to seeding data
            await initializer.SeedAsync();
        }
    }
}

public interface IDatabaseInitializer
{
    Task InitializeAsync();
    Task SeedAsync();
    Task TrySeedAsync();
}


public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly EscapeRoomUnityContext _context;
    private readonly AppSettings _appSettings;
    private readonly IFirebaseService _firebaseService;

    public DatabaseInitializer(EscapeRoomUnityContext context,
        IOptionsMonitor<AppSettings> asMonitor,
        IFirebaseService firebaseService)
    {
        _context = context;
        _appSettings = asMonitor.CurrentValue;
        _firebaseService = firebaseService;
    }

    public async Task InitializeAsync()
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
                Console.WriteLine("Migrations have already been applied. Skip migrations process.");
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