using System.Net.Http.Headers;
using ExpertBridge.Api.HttpClients;
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

        builder.Services
            .AddScoped<GroqApiChatCompletionClient>()
            .AddScoped<GroqLlmTextProvider>()
            .AddScoped<GroqPostTaggingService>()
            .AddScoped<TagProcessorService>()
            .AddScoped<GroqInappropriateLanguageDetectionService>()
            ;

        // Adding an HttpClient to a service should take place after registering this
        // service with the DI frist.
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
    ///     The modified <see cref="WebApplicationBuilder" /> instance.
    /// </returns>
    /// <remarks>
    ///     This method initializes an HTTP client with a base address and authorization header
    ///     using the API key from the <see cref="GroqSettings" /> configuration.
    /// </remarks>
    public static WebApplicationBuilder AddGroqHttpClient(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection(GroqSettings.Section).Get<GroqSettings>()!;

        builder.Services.AddHttpClient<GroqApiChatCompletionClient>(client =>
        {
            client.BaseAddress = new Uri(GroqApiEndpoints.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        });

        return builder;
    }
}
