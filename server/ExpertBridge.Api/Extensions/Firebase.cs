// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Application.Services;
using ExpertBridge.Api.Core.Interfaces.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        builder.Services.AddHttpClient<IFirebaseAuthService, FirebaseAuthService>((sp, httpClient) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var uri = configuration["Authentication:Firebase:TokenUri"]!;
            httpClient.BaseAddress = new Uri(uri);
        });
    }

    /// <summary>
    ///     Adds the Firebase authentication service to the application builder.
    /// </summary>
    /// <param name="builder">builder — The WebApplicationBuilder to add the Firebase authentication services to</param>
    public static void AddFirebaseAuthentication(this WebApplicationBuilder builder)
    {
        var projectId = builder.Configuration["Firebase:ProjectId"]!;
        var issuer = builder.Configuration["Authentication:Firebase:Issuer"]!;
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.Authority = issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true
                };
                options.RequireHttpsMetadata = false;
            })
            ;
        builder.Services.AddAuthorization();
    }
}
