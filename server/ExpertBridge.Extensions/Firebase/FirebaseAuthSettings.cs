// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Extensions.Firebase;

/// <summary>
/// Represents JWT authentication configuration settings for Firebase Authentication in the ExpertBridge application.
/// Defines token validation parameters for verifying Firebase ID tokens in API requests.
/// </summary>
/// <remarks>
/// These settings are loaded from the "Authentication:Firebase" configuration section and used to configure
/// JWT Bearer authentication middleware for validating Firebase-issued ID tokens.
///
/// Firebase Authentication flow:
/// 1. Client authenticates with Firebase (email/password, Google, etc.)
/// 2. Client receives Firebase ID token (JWT)
/// 3. Client includes token in Authorization header of API requests
/// 4. Server validates token using these settings
/// 5. Server extracts user ProviderId from token claims
///
/// The issuer and audience settings ensure tokens are issued by Firebase for the ExpertBridge project.
/// </remarks>
public sealed class FirebaseAuthSettings
{
    /// <summary>
    /// Gets the configuration section name for Firebase authentication settings.
    /// </summary>
    public const string Section = "Authentication:Firebase";

    /// <summary>
    /// Gets or sets the expected JWT issuer URL for Firebase ID tokens (e.g., "https://securetoken.google.com/expert-bridge").
    /// Used to validate that tokens are issued by Firebase Authentication for the correct project.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected JWT audience (Firebase project ID) for validating token recipients.
    /// Ensures tokens are intended for the ExpertBridge application.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URI for obtaining Firebase authentication tokens.
    /// Used by services that need to interact with Firebase authentication REST API.
    /// </summary>
    public string TokenUri { get; set; } = string.Empty;
}
