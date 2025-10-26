// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ExpertBridge.Extensions.Firebase;

/// <summary>
///     Provides extension methods for integrating Firebase services into the ExpertBridge application.
///     Configures Firebase Admin SDK, Authentication, and Cloud Messaging for user management and push notifications.
/// </summary>
internal static class Firebase
{
    /// <summary>
    ///     Registers Firebase Admin SDK services including Firebase App, Authentication, and Cloud Messaging.
    ///     Initializes the Firebase app with service account credentials from a JSON file.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure Firebase services for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method:
    ///     - Creates a Firebase app instance using Google credentials from "FirebaseOAuthCredentialsExpertBridge.json"
    ///     - Registers FirebaseAuth for server-side user management and token verification
    ///     - Registers FirebaseMessaging for sending push notifications to mobile clients
    ///     All three services are registered as singletons and available for dependency injection throughout the application.
    /// </remarks>
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
    ///     Registers a typed HttpClient for IFirebaseAuthService configured with the Firebase authentication token endpoint.
    ///     Enables custom Firebase authentication operations via REST API calls.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure the HttpClient for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method configures an HttpClient specifically for Firebase Authentication REST API operations.
    ///     The client's base address is set to FirebaseSettings.AuthenticationTokenUri for making custom authentication
    ///     requests.
    ///     The typed client is injected into IFirebaseAuthService implementations for token refresh, user management, and
    ///     other Firebase auth operations.
    /// </remarks>
    public static TBuilder AddHttpClientForFirebaseService<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var settings = builder.Configuration.GetSection("Firebase").Get<FirebaseSettings>()!;
        builder.Services.AddHttpClient<IFirebaseAuthService>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(settings.AuthenticationTokenUri);
        });

        return builder;
    }

    /// <summary>
    ///     Configures JWT Bearer authentication using Firebase ID tokens for API authorization.
    ///     Sets up token validation parameters including issuer, audience, and lifetime validation.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure Firebase authentication for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method configures ASP.NET Core authentication middleware to validate Firebase-issued JWT tokens:
    ///     **Token Validation:**
    ///     - Validates issuer matches Firebase's secure token service URL for the project
    ///     - Validates audience matches the Firebase project ID
    ///     - Validates token lifetime (expiration) to prevent replay attacks
    ///     - Includes detailed error information for debugging authentication issues
    ///     **Authentication Flow:**
    ///     1. Client authenticates with Firebase (email/password, Google OAuth, etc.)
    ///     2. Firebase returns an ID token (JWT)
    ///     3. Client includes token in Authorization: Bearer header
    ///     4. Middleware validates token using configured parameters
    ///     5. User's ProviderId is extracted from token claims for user identification
    ///     Also registers authorization services for applying [Authorize] attributes to controllers and actions.
    ///     HTTPS metadata validation is disabled to support development environments (should be enabled in production).
    /// </remarks>
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
