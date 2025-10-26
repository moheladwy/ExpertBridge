// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Data;

/// <summary>
///     Provides extension methods for configuring Entity Framework Core database services in the ExpertBridge application.
///     Contains methods for registering the PostgreSQL database context with pgvector support, connection resilience, and
///     soft delete interceptors.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Adds the ExpertBridge database context to the service collection with PostgreSQL, pgvector extension, retry logic,
    ///     and soft delete interceptor.
    ///     Configures Entity Framework Core with connection string from configuration, enables automatic retries on transient
    ///     failures, and applies soft delete pattern interception.
    /// </summary>
    /// <param name="services">The service collection to add the database context to.</param>
    /// <param name="configuration">
    ///     The configuration instance containing the PostgreSQL connection string under
    ///     "ConnectionStrings:Postgresql".
    /// </param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    ///     This method configures:
    ///     - PostgreSQL with Npgsql provider and pgvector extension for semantic search with embeddings
    ///     - Connection resilience with 5 retry attempts over 60 seconds for transient failures
    ///     - Soft delete interceptor to automatically handle ISoftDeletable entities during delete operations
    /// </remarks>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgresql")!;
        services.AddDbContext<ExpertBridgeDbContext>(options =>
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
}
