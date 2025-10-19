// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Host.Resources;

/// <summary>
/// Provides methods for configuring and accessing Seq resources within an
/// application using a distributed application builder.
/// </summary>
internal static class Seq
{
    /// <summary>
    /// Configures and retrieves the Seq resource for a distributed application builder.
    /// </summary>
    /// <param name="builder">The distributed application builder used to configure the Seq resource.</param>
    /// <returns>An <see cref="IResourceBuilder{T}"/> configured for the Seq resource.</returns>
    public static IResourceBuilder<SeqResource> GetSeqResource(this IDistributedApplicationBuilder builder)
    {
        var seq = builder
            .AddSeq("Seq", 4002)
            .WithContainerName("expertbridge-seq")
            .WithDataVolume("expertbridge-seq-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOtlpExporter()
            .WithExternalHttpEndpoints();

        return seq;
    }
}
