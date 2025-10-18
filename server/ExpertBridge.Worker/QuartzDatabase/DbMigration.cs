// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Worker.QuartzDatabase;

/// <summary>
///     Provides functionality for database migration operations at application startup.
/// </summary>
internal static class DbMigration
{
    /// <summary>
    ///     Automatically migrates the database at startup.
    /// </summary>
    /// <param name="app">
    ///     The WebApplication instance to use for the migration.
    /// </param>
    public static async Task ApplyMigrationAtStartup(this IHost app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ExpertBridgeQuartzDbContext>();
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
