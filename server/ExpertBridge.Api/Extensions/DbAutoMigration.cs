using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Extensions;

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
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            var dbContext = services.GetRequiredService<ExpertBridgeDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
