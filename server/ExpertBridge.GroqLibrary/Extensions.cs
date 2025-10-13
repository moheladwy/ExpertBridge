// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http.Headers;
using ExpertBridge.GroqLibrary.Clients;
using ExpertBridge.GroqLibrary.Providers;
using ExpertBridge.GroqLibrary.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.GroqLibrary;

public static class Extensions
{
    /// <summary>
    ///     Adds Groq API-related services to the dependency injection container of the application.
    /// </summary>
    /// <param name="builder">
    ///     The builder to add the Groq API-related services to.
    /// </param>
    /// <remarks>
    ///     This method registers the <see cref="GroqApiChatCompletionClient" /> and
    ///     <see cref="GroqLlmTextProvider" /> services into the application's service collection.
    ///     These services are essential for enabling Groq API functionalities.
    /// </remarks>
    /// <seealso cref="GroqApiChatCompletionClient" />
    /// <seealso cref="GroqLlmTextProvider" />
    public static TBuilder AddGroqApiServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services
            .AddScoped<GroqApiChatCompletionClient>()
            .AddScoped<GroqLlmTextProvider>()
            ;

        builder.AddGroqHttpClient();
        return builder;
    }


    /// <summary>
    ///     Configures and registers the HTTP client used for interacting with the Groq API.
    /// </summary>
    /// <param name="builder">
    ///     The web application builder to which the Groq HTTP client will be added.
    /// </param>
    /// <returns>
    ///     The modified <see cref="TBuilder" /> instance.
    /// </returns>
    /// <remarks>
    ///     This method initializes an HTTP client with a base address and authorization header
    ///     using the API key from the <see cref="GroqSettings" /> configuration.
    /// </remarks>
    public static TBuilder AddGroqHttpClient<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var settings = builder.Configuration
            .GetSection(GroqSettings.Section)
            .Get<GroqSettings>()!;

        builder.Services.AddHttpClient<GroqApiChatCompletionClient>(client =>
        {
            client.BaseAddress = new Uri(GroqApiEndpoints.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        }).AddStandardResilienceHandler();

        return builder;
    }
}
