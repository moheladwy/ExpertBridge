// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Application.Services;
using ExpertBridge.Api.Settings;
using ExpertBridge.Api.Core.Interfaces.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExpertBridge.Api.Extensions;

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
    public static void AddFirebaseApp(this WebApplicationBuilder builder)
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
    }

    /// <summary>
    ///     Adds the HttpClient service for the Firebase service.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the HttpClient service to.
    /// </param>
    public static void AddHttpClientForFirebaseService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<HttpClient>((sp, httpClient) =>
        {
            var settings = sp.GetRequiredService<IOptionsSnapshot<FirebaseSettings>>().Value;
            httpClient.BaseAddress = new Uri(settings.AuthenticationTokenUri);
        });
    }

    /// <summary>
    ///     Adds the Firebase authentication service to the application builder.
    /// </summary>
    /// <param name="builder">builder — The WebApplicationBuilder to add the Firebase authentication services to</param>
    public static void AddFirebaseAuthentication(this WebApplicationBuilder builder)
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
    }
}
