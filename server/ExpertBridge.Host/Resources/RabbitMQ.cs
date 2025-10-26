// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Host.Resources;

/// <summary>
///     Provides functionality for configuring and retrieving RabbitMQ server resources
///     within a distributed application context.
/// </summary>
internal static class RabbitMq
{
    /// <summary>
    ///     Configures and retrieves the RabbitMQ server resource for a distributed application.
    ///     This method sets up RabbitMQ with predefined username and password parameters,
    ///     default port, data volume, management plugin, and telemetry exporter.
    /// </summary>
    /// <param name="builder">
    ///     The <see cref="IDistributedApplicationBuilder" /> used to configure the application and manage its resources.
    /// </param>
    /// <returns>
    ///     An <see cref="IResourceBuilder{RabbitMQServerResource}" /> representing the configured RabbitMQ server resource.
    /// </returns>
    public static IResourceBuilder<RabbitMQServerResource> GetRabbitMqResource(
        this IDistributedApplicationBuilder builder)
    {
        var userName = builder
            .AddParameterFromConfiguration("RabbitMq-Username", "RabbitMQ:Username");
        var password = builder
            .AddParameterFromConfiguration("RabbitMq-Password", "RabbitMQ:Password");
        const int port = 5672;

        var rabbitMq = builder
            .AddRabbitMQ("rabbitmq", userName, password, port)
            .WithContainerName("expertbridge-rabbitmq")
            .WithDataVolume("expertbridge-rabbitmq-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithManagementPlugin()
            .WithOtlpExporter()
            .WithExternalHttpEndpoints();

        return rabbitMq;
    }
}
