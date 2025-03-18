using ExpertBridge.Api.Core.Entities.User;
using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.Extensions;

internal static class DbMigration
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
        catch (InvalidOperationException e)
        {
            Log.Error(e, "An error occurred while getting the database context.");
        }
        catch (OperationCanceledException e)
        {
            Log.Error(e, "An error occurred while migrating the database.");
        }
    }
}
