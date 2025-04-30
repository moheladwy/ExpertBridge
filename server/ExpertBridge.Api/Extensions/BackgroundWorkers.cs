// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.BackgroundServices;

namespace ExpertBridge.Api.Extensions
{
    public static class BackgroundWorkers
    {
        public static WebApplicationBuilder AddBackgroundWorkers(this WebApplicationBuilder builder)
        {
            // The name HandlerWorker indicates that the class is a worker that handles messages from a channel.
            // AKA. event-driven. 

            builder.Services
                .AddHostedService<S3CleaningWorker>()
                .AddHostedService<PeriodicPostTaggingCleanerWorker>()
                .AddHostedService<PostCreatedHandlerWorker>()
                .AddHostedService<UserInterestsUpdatedHandlerWorker>()
                .AddHostedService<PostEmbeddingHandlerWorker>()
                ;

            return builder;
        }
    }
}
