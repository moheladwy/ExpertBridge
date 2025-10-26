// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExpertBridge.Data.DatabaseContexts;

/// <summary>
///     Provides a factory for creating instances of <see cref="ExpertBridgeDbContext" /> at design time for Entity
///     Framework Core tooling.
///     Used by EF Core migration commands (dotnet ef migrations add, dotnet ef database update) to create a database
///     context without running the application.
/// </summary>
/// <remarks>
///     This factory configures the database context with:
///     - Configuration loaded from appsettings.json, appsettings.Development.json, environment variables, and user secrets
///     - PostgreSQL connection string from "ConnectionStrings:Postgresql" configuration section
///     - Pgvector extension support for semantic search with 1024-dimensional vector embeddings
///     - Connection resilience with 10 retry attempts over 60 seconds for transient database failures
///     The factory is automatically discovered and used by Entity Framework Core CLI tools during migration operations.
/// </remarks>
public class ExpertBridgeDbContextFactory : IDesignTimeDbContextFactory<ExpertBridgeDbContext>
{
    /// <summary>
    ///     Creates a new instance of <see cref="ExpertBridgeDbContext" /> configured for design-time operations.
    ///     Loads configuration from multiple sources and constructs a database context with PostgreSQL and pgvector support.
    /// </summary>
    /// <param name="args">Command-line arguments passed from EF Core tooling (typically unused).</param>
    /// <returns>A configured instance of <see cref="ExpertBridgeDbContext" /> ready for migration operations.</returns>
    /// <remarks>
    ///     Configuration is loaded in the following order (later sources override earlier ones):
    ///     1. appsettings.json base configuration
    ///     2. appsettings.Development.json environment-specific settings
    ///     3. User secrets for sensitive data (connection strings, API keys)
    ///     4. ASPNETCORE_ENVIRONMENT environment variable
    ///     This method is called automatically by EF Core CLI commands and should not be invoked directly in application code.
    /// </remarks>
    public ExpertBridgeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT")
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddUserSecrets<ExpertBridgeDbContext>()
            .Build();

        var connectionString = configuration.GetConnectionString("Postgresql")!;
        var optionsBuilder = new DbContextOptionsBuilder<ExpertBridgeDbContext>();
        optionsBuilder.UseNpgsql(connectionString, o =>
        {
            o.UseVector();
            o.EnableRetryOnFailure(
                10,
                TimeSpan.FromSeconds(60),
                null);
        });
        return new ExpertBridgeDbContext(optionsBuilder.Options);
    }
}
