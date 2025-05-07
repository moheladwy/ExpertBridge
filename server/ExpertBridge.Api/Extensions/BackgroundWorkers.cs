// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.BackgroundServices.Handlers;
using ExpertBridge.Api.BackgroundServices.PeriodicJobs;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides an extension method for registering background workers to a WebApplicationBuilder.
/// </summary>
public static class BackgroundWorkers
{
    /// <summary>
    ///     Configures and registers background workers into the service collection of the application builder.
    ///     These workers execute background tasks for the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> instance to configure.</param>
    /// <returns>The modified <see cref="WebApplicationBuilder" /> with the registered background workers.</returns>
    public static WebApplicationBuilder AddBackgroundWorkers(this WebApplicationBuilder builder)
    {
        // The name HandlerWorker indicates that the class is a worker that handles messages from a channel.
        // AKA. event-driven.

        builder.Services
            .AddHostedService<S3CleaningPeriodicWorker>()
            .AddHostedService<PostTaggingPeriodicWorker>()
            .AddHostedService<PostEmbeddingPeriodicWorker>()
            .AddHostedService<UserInterestUpdaterPeriodicWorker>()
            .AddHostedService<ContentModerationPeriodicWorker>()
            .AddHostedService<PostProcessingPipelineHandlerWorker>()
            .AddHostedService<PostTaggingHandlerWorker>()
            .AddHostedService<PostEmbeddingHandlerWorker>()
            .AddHostedService<UserInterestsUpdatedHandlerWorker>()
            .AddHostedService<InappropriatePostDetectionHandlerWorker>()
            .AddHostedService<InappropriateCommentDetectionHandlerWorker>()
            ;

        return builder;
    }
}
