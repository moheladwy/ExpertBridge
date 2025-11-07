// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Database;

/// <summary>
///     Extension methods for configuring the Admin database context and applying database migrations.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Adds the Admin database context to the service collection with PostgreSQL, pgvector extension, retry logic,
    ///     and soft delete interceptor.
    /// </summary>
    /// <param name="services">The service collection to add the database context to.</param>
    /// <param name="configuration">
    ///     The configuration instance containing the PostgreSQL connection string under
    ///     "ConnectionStrings:AdminDb".
    /// </param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    ///     Configures the <see cref="AdminDbContext" /> with:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>PostgreSQL using Npgsql provider with pgvector extension for semantic search capabilities</description>
    ///         </item>
    ///         <item>
    ///             <description>Connection resilience with 5 retry attempts over 60 seconds for transient failures</description>
    ///         </item>
    ///         <item>
    ///             <description>Soft delete interceptor for automatic ISoftDeletable entity handling</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public static IServiceCollection AddAdminDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AdminDb")!;
        services.AddDbContext<AdminDbContext>(options =>
        {
            options.UseNpgsql(connectionString, o =>
            {
                o.UseVector();
                o.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(60),
                    null);
            });
            options.AddInterceptors(new SoftDeleteInterceptor());
        });
        return services;
    }

    /// <summary>
    ///     Automatically applies pending database migrations at application startup.
    /// </summary>
    /// <param name="app">The WebApplication instance to use for applying migrations.</param>
    /// <returns>A task representing the asynchronous migration operation.</returns>
    /// <remarks>
    ///     This method creates a new service scope to retrieve the <see cref="AdminDbContext" /> and applies all pending
    ///     migrations. If an error occurs during migration, it is logged as an error but does not throw, allowing the
    ///     application to continue startup.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the database context cannot be retrieved from the service provider.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the migration operation is canceled, such as during application shutdown.
    /// </exception>
    public static async Task ApplyMigrationAtStartup(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var dbContext = services.GetRequiredService<AdminDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e, "An error occurred while getting the database context.");
        }
        catch (OperationCanceledException e)
        {
            logger.LogError(e, "An error occurred while migrating the database.");
        }
    }
}
