// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.Postgres;

namespace ExpertBridge.Host.Resources;

/// <summary>
///     Provides extension methods for configuring Postgresql resources in a distributed application setup.
/// </summary>
internal static class Postgresql
{
    /// <summary>
    ///     Configures and provides a resource builder for a Postgres server resource, integrated with the application
    ///     builder. This includes database and PgAdmin setup with predefined configurations for container name, lifetime,
    ///     data volume, environment variables, and OTLP exporter.
    /// </summary>
    /// <param name="builder">The distributed application builder used to configure and manage distributed resources.</param>
    /// <returns>
    ///     A resource builder for the Postgres server resource, ready for deployment and integration within the
    ///     application.
    /// </returns>
    public static IResourceBuilder<PostgresServerResource> GetPostgresqlResource(
        this IDistributedApplicationBuilder builder)
    {
        var postgresqlUserName = builder
            .AddParameterFromConfiguration("Postgres-Username", "Postgres:Username");
        var postgresqlPassword = builder
            .AddParameterFromConfiguration("Postgres-Password", "Postgres:Password");

        var postgresql = builder
            .AddPostgres("Postgres", postgresqlUserName, postgresqlPassword, 5432)
            .WithImage("pgvector/pgvector", "pg18")
            .WithContainerName("expertbridge-postgres")
            .WithDataVolume("expertbridge-postgres-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOtlpExporter();


        postgresql = postgresql.WithPgAdmin(cfg => { cfg.GetPgadmin(postgresql); });
        postgresqlUserName = postgresqlUserName.WithParentRelationship(postgresql);
        postgresqlPassword = postgresqlPassword.WithParentRelationship(postgresql);

        return postgresql;
    }

    /// <summary>
    ///     Configures and provides a resource builder for a PgAdmin container resource, integrated with the specified
    ///     Postgres server resource. This includes predefined configurations such as image version, container name,
    ///     port mapping, lifetime, and environment variables.
    /// </summary>
    /// <param name="pgadmin">The resource builder for the PgAdmin container resource being configured.</param>
    /// <param name="postgresql">
    ///     The resource builder for the Postgres server resource to be associated with this PgAdmin
    ///     container.
    /// </param>
    /// <returns>
    ///     A resource builder for the PgAdmin container resource, integrated with the specified Postgres server resource,
    ///     and ready for deployment.
    /// </returns>
    private static IResourceBuilder<PgAdminContainerResource> GetPgadmin(
        this IResourceBuilder<PgAdminContainerResource> pgadmin,
        IResourceBuilder<PostgresServerResource> postgresql)
    {
        return pgadmin
            .WithImage("dpage/pgadmin4", "latest")
            .WithContainerName("expertbridge-pgadmin")
            .WithHostPort(5050)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@admin.com")
            .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "admin@admin")
            .WithEnvironment("PGADMIN_CONFIG_FORCE_SSL", "True")
            .WithEnvironment("PGADMIN_CONFIG_ENHANCED_COOKIE_PROTECTION", "False")
            .WithEnvironment("PGADMIN_CONFIG_WTF_CSRF_CHECK_DEFAULT", "False")
            .WithEnvironment("PGADMIN_CONFIG_WTF_CSRF_ENABLED", "False")
            .WithParentRelationship(postgresql)
            .WithExternalHttpEndpoints();
    }
}
