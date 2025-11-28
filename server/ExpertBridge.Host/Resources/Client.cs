// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.JavaScript;

namespace ExpertBridge.Host.Resources;

/// <summary>
///     Provides extension methods for configuring a <see cref="IResourceBuilder{T}" />
///     with predefined environment variables and application-specific parameters.
/// </summary>
internal static class Client
{
    /// <summary>
    ///     Configures an <see cref="IResourceBuilder{T}" /> instance with predefined environment variables
    ///     and application-specific parameters obtained from the <see cref="IDistributedApplicationBuilder" />.
    /// </summary>
    /// <param name="builder">
    ///     The <see cref="IResourceBuilder{T}" /> to be configured.
    /// </param>
    /// <param name="distributedApplicationBuilder">
    ///     The <see cref="IDistributedApplicationBuilder" /> used to retrieve configuration parameters
    ///     for setting environment variables.
    /// </param>
    /// <returns>
    ///     The configured <see cref="IResourceBuilder{T}" /> instance with the predefined environment variables.
    /// </returns>
    public static IResourceBuilder<ViteAppResource> WithEnvironments(
        this IResourceBuilder<ViteAppResource> builder,
        IDistributedApplicationBuilder distributedApplicationBuilder)
    {
        var apiUrl = distributedApplicationBuilder
            .AddParameter("api-url", "http://localhost:5027").WithParentRelationship(builder);
        var indexDbName = distributedApplicationBuilder
            .AddParameterFromConfiguration("indexed-db-name", "client:indexed-db-name", true)
            .WithParentRelationship(builder);
        var indexDbVersion = distributedApplicationBuilder
            .AddParameterFromConfiguration("indexed-db-version", "client:indexed-db-version", true)
            .WithParentRelationship(builder);
        var apiKey = distributedApplicationBuilder
            .AddParameterFromConfiguration("api-key", "client:api-key", true)
            .WithParentRelationship(builder);
        var authDomain = distributedApplicationBuilder
            .AddParameterFromConfiguration("auth-domain", "client:auth-domain", true).WithParentRelationship(builder);
        var projectId = distributedApplicationBuilder
            .AddParameterFromConfiguration("project-id", "client:project-id", true).WithParentRelationship(builder);
        var storageBucket = distributedApplicationBuilder
            .AddParameterFromConfiguration("storage-bucket", "client:storage-bucket", true)
            .WithParentRelationship(builder);
        var messagingSenderId = distributedApplicationBuilder
            .AddParameterFromConfiguration("messaging-sender-id", "client:messaging-sender-id", true)
            .WithParentRelationship(builder);
        var appId = distributedApplicationBuilder
            .AddParameterFromConfiguration("app-id", "client:app-id", true).WithParentRelationship(builder);
        var measurementId = distributedApplicationBuilder
            .AddParameterFromConfiguration("measurement-id", "client:measurement-id", true)
            .WithParentRelationship(builder);
        return builder
            .WithEnvironment("VITE_SERVER_URL", apiUrl)
            .WithEnvironment("VITE_INDEXED_DB_NAME", indexDbName)
            .WithEnvironment("VITE_INDEXED_DB_VERSION", indexDbVersion)
            .WithEnvironment("VITE_API_KEY", apiKey)
            .WithEnvironment("VITE_AUTH_DOMAIN", authDomain)
            .WithEnvironment("VITE_PROJECT_ID", projectId)
            .WithEnvironment("VITE_STORAGE_BUCKET", storageBucket)
            .WithEnvironment("VITE_MESSAGING_SENDER_ID", messagingSenderId)
            .WithEnvironment("VITE_APP_ID", appId)
            .WithEnvironment("VITE_MEAUSUREMENT_ID", measurementId)
            .WithEnvironment("VITE_ENABLE_DEBUG_LOGGING", "true")
            .WithEnvironment("VITE_API_TIMEOUT", "60000")
            .WithEnvironment("VITE_MAX_API_RETRIES", "3")
            .WithEnvironment("VITE_ENABLE_TOKEN_MONITOR", "true")
            .WithEnvironment("VITE_ENABLE_AUTH_MONITOR", "true")
            .WithEnvironment("VITE_ENABLE_PERFORMANCE_MONITORING", "true")
            .WithEnvironment("VITE_ENABLE_REACT_DEVTOOLS", "true")
            .WithEnvironment("VITE_ENABLE_REDUX_DEVTOOLS", "true")
            .WithEnvironment("PORT", "5173");
    }
}
