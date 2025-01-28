using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Database;

internal static class DbAutoMigration
{
    /// <summary>
    ///     Automatically migrates the database at startup.
    /// </summary>
    /// <param name="app">
    ///     The WebApplication instance to use for the migration.
    /// </param>
    public static async Task UseAutoMigrationAtStartup(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            ExpertBridgeDbContext dbContext = services.GetRequiredService<ExpertBridgeDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
