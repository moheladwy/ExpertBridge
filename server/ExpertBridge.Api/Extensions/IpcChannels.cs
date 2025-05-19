// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for adding inter-process communication (IPC) channels to the dependency injection
///     container.
/// </summary>
public static class IpcChannels
{
    /// <summary>
    ///     Configures and registers IPC (Inter-Process Communication) channels for handling messages related to post creation,
    ///     user interest updates,
    ///     and embedding posts. This method adds unbounded channels and registers them as singleton services in the dependency
    ///     injection container.
    /// </summary>
    /// <param name="builder">The application builder used to configure the web application and services.</param>
    /// <returns>The updated <see cref="WebApplicationBuilder" /> instance with the IPC channels configured.</returns>
    public static WebApplicationBuilder AddIpcChannels(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton(_ => Channel.CreateUnbounded<TagPostMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<UserInterestsUpdatedMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<EmbedPostMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<DetectInappropriatePostMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<DetectInappropriateCommentMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<AcknowledgePostProcessingMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<PostProcessingPipelineMessage>())
            .AddSingleton(_ => Channel.CreateUnbounded<UserInterestsProsessingMessage>())
            ;

        return builder;
    }
}
