// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.BackgroundServices;

namespace ExpertBridge.Api.Extensions
{
    public static class BackgroundWorkers
    {
        public static WebApplicationBuilder AddBackgroundWorkers(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddHostedService<S3CleaningWorker>()
                .AddHostedService<PostCreatedHandlerWorker>()
                .AddHostedService<UserInterestsUpdatedHandlerWorker>()
                ;

            return builder;
        }
    }
}
