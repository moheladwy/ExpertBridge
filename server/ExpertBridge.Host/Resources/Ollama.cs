// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Host.Resources;

/// <summary>
/// Represents the Ollama functionality within the ExpertBridge Host resources.
/// </summary>
/// <remarks>
/// This static class provides methods to configure and retrieve an Ollama resource
/// for distributed applications. It allows specifying container settings, data volumes,
/// GPU support, OTLP exporters, and model configurations.
/// </remarks>
internal static class Ollama
{
    /// <summary>
    /// Configures and retrieves an Ollama resource within a distributed application builder.
    /// </summary>
    /// <param name="builder">The application builder used for configuring the distributed application.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{T}"/> instance for the Ollama model resource configured with specified settings.
    /// </returns>
    public static IResourceBuilder<OllamaModelResource> GetOllamaResource(this IDistributedApplicationBuilder builder)
    {
        var ollama = builder
            .AddOllama("ollama", 11434)
            .WithContainerName("expertbridge-ollama")
            .WithDataVolume("expertbridge-ollama-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithGPUSupport()
            .WithOtlpExporter()
            .WithExternalHttpEndpoints()
            .AddModel("snowflake-arctic-embed2:latest");

        return ollama;
    }
}
