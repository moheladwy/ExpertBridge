// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using Serilog;

namespace ExpertBridge.Extensions.Resilience;

public static class ResiliencePipeline
{
    public static TBuilder AddResiliencePipeline<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddResiliencePipeline(ResiliencePipelines.MalformedJsonModelResponse, static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<JsonException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = context =>
                {
                    // Log the retry attempt
                    Log.Information("Retrying due to a malformed json in model response. Attempt");
                    return ValueTask.CompletedTask;
                }
            });

            // See: https://www.pollydocs.org/strategies/timeout.html
            builder.AddTimeout(TimeSpan.FromSeconds(90));
        });
        return builder;
    }
}
