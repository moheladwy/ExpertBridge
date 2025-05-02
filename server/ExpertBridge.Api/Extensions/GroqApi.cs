using System.Net.Http.Headers;
using ExpertBridge.Api.Services;
using ExpertBridge.GroqLibrary.Clients;
using ExpertBridge.GroqLibrary.Providers;
using ExpertBridge.GroqLibrary.Settings;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     The GroqApi class provides an extension method for adding Groq API-related services to the dependency injection
///     container
///     of an ASP.NET Core application.
/// </summary>
/// <remarks>
///     This static class is used to register services that are essential for integrating the Groq API functionalities into
///     a web application.
///     Specifically, it adds the <see cref="GroqApiChatCompletionClient" /> and <see cref="GroqLlmTextProvider" />
///     services to the ASP.NET Core
///     service collection.
/// </remarks>
/// <seealso cref="GroqApiChatCompletionClient" />
/// <seealso cref="GroqLlmTextProvider" />
public static class GroqApi
{
    /// <summary>
    ///     Adds Groq API-related services to the dependency injection container of the application.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the Groq API-related services to.
    /// </param>
    /// <remarks>
    ///     This method registers the <see cref="GroqApiChatCompletionClient" /> and
    ///     <see cref="GroqLlmTextProvider" /> services into the application's service collection.
    ///     These services are essential for enabling Groq API functionalities.
    /// </remarks>
    /// <seealso cref="GroqApiChatCompletionClient" />
    /// <seealso cref="GroqLlmTextProvider" />
    public static WebApplicationBuilder AddGroqApiServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<GroqApiChatCompletionClient>(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(GroqSettings.Section);
            return new GroqApiChatCompletionClient(httpClient);
        })
        .AddScoped<GroqLlmTextProvider>()
        .AddScoped<GroqPostTaggingService>()
        .AddScoped<TagProcessorService>()
        .AddScoped<NSFWDetectionService>()
        ;
        return builder;
    }

    /// <summary>
    /// Adds an HTTP client configured for Groq API endpoints to the application's dependency injection container.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder" /> used to configure the application's service container.
    /// </param>
    /// <returns>
    /// The modified <see cref="WebApplicationBuilder" /> with the Groq API HTTP client configured.
    /// </returns>
    /// <remarks>
    /// This method registers an <see cref="IHttpClientFactory" /> named after the Groq settings section,
    /// pre-configured with the Groq API base URL and the authorization header using the Groq API key.
    /// </remarks>
    /// <seealso cref="GroqSettings" />
    /// <seealso cref="GroqApiEndpoints" />
    public static WebApplicationBuilder AddGroqHttpClientFactory(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection(GroqSettings.Section).Get<GroqSettings>()!;

        builder.Services.AddHttpClient(GroqSettings.Section, client =>
        {
            client.BaseAddress = new Uri(GroqApiEndpoints.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        });

        return builder;
    }
}
