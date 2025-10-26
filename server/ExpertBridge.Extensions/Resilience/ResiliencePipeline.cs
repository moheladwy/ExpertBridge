// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using Serilog;

namespace ExpertBridge.Extensions.Resilience;

/// <summary>
///     Provides extension methods for configuring Polly resilience pipelines in the ExpertBridge application.
///     Implements retry and timeout strategies for handling transient failures in AI/LLM API interactions.
/// </summary>
public static class ResiliencePipeline
{
    /// <summary>
    ///     Registers a resilience pipeline for handling malformed JSON responses from AI model APIs (Groq, Ollama).
    ///     Configures exponential backoff retry strategy and timeout policy for JSON parsing failures.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure resilience pipelines for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This pipeline addresses a common issue with AI/LLM APIs where responses may contain malformed JSON due to:
    ///     - Model hallucination or incomplete generation
    ///     - Response truncation from token limits
    ///     - Streaming response issues
    ///     - Network interruptions during response transfer
    ///     **Retry Strategy:**
    ///     - Handles JsonException specifically (malformed JSON parsing errors)
    ///     - Maximum 3 retry attempts before failing
    ///     - Exponential backoff starting at 2 seconds (2s, 4s, 8s)
    ///     - Jitter enabled to prevent thundering herd effect
    ///     - Logs each retry attempt for monitoring
    ///     **Timeout Policy:**
    ///     - 90-second timeout for AI model API calls
    ///     - Prevents indefinite hangs on slow or unresponsive model endpoints
    ///     - Allows sufficient time for complex AI processing (tagging, moderation, embedding)
    ///     Used by:
    ///     - Post tagging service (Groq API for multilingual tag generation)
    ///     - Content moderation service (Groq API for safety analysis)
    ///     - Embedding generation service (Ollama for vector embeddings)
    ///     Access the pipeline via ResiliencePipelines.MalformedJsonModelResponse constant.
    /// </remarks>
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
