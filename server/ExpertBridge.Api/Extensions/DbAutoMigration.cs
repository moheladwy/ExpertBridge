using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.Extensions;

internal static class DbAutoMigration
{
    /// <summary>
    ///     Automatically migrates the database at startup.
    /// </summary>
    /// <param name="app">
    ///     The WebApplication instance to use for the migration.
    /// </param>
    public static async Task ApplyMigrationAtStartup(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ExpertBridgeDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occurred while migrating the database.");
        }
    }
}
