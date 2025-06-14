using ExpertBridge.Core.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExpertBridge.Extensions.Firebase;

/// <summary>
///     Provides extension methods for adding Firebase services to the application builder.
/// </summary>
internal static class Firebase
{
    /// <summary>
    ///     Adds the Firebase app, auth, and messaging services to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the Firebase services to.
    /// </param>
    public static TBuilder AddFirebaseApp<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("FirebaseOAuthCredentialsExpertBridge.json")
        });
        var firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
        var firebaseMessaging = FirebaseMessaging.GetMessaging(firebaseApp);

        builder.Services.AddSingleton(firebaseApp);
        builder.Services.AddSingleton(firebaseAuth);
        builder.Services.AddSingleton(firebaseMessaging);

        return builder;
    }

    /// <summary>
    ///     Adds the HttpClient service for the Firebase service.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the HttpClient service to.
    /// </param>
    public static TBuilder AddHttpClientForFirebaseService<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var settings = builder.Configuration.GetSection("Firebase").Get<FirebaseSettings>()!;
        builder.Services.AddHttpClient<IFirebaseAuthService>(httpClient=>
        {
            httpClient.BaseAddress = new Uri(settings.AuthenticationTokenUri);
        });

        return builder;
    }

    /// <summary>
    ///     Adds the Firebase authentication service to the application builder.
    /// </summary>
    /// <param name="builder">builder — The WebApplicationBuilder to add the Firebase authentication services to</param>
    public static TBuilder AddFirebaseAuthentication<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var config = builder.Configuration.GetSection("Firebase").Get<FirebaseSettings>();
        var authSettings = builder.Configuration.GetSection("Authentication:Firebase").Get<FirebaseAuthSettings>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.Authority = authSettings.Issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.ProjectId,
                    ValidateLifetime = true
                };
                options.RequireHttpsMetadata = false;
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
