using ExpertBridge.Api.Services;
using ExpertBridge.GroqLibrary.Clients;
using ExpertBridge.GroqLibrary.Providers;
using ExpertBridge.GroqLibrary.Settings;
using Microsoft.Extensions.Options;

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
        builder.Configuration.GetSection(GroqSettings.Section).Bind(new GroqSettings());

        builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection(GroqSettings.Section));
        builder.Services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<GroqSettings>>().Value;
            return new GroqLlmTextProvider(settings.ApiKey, settings.Model);
        });

        builder.Services
            .AddScoped<GroqPostTaggingService>()
            .AddScoped<TagProcessorService>()
            ;

        return builder;
    }
}
