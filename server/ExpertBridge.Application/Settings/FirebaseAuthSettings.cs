namespace ExpertBridge.Application.Settings;

/// <summary>
///     Configuration settings for Firebase Authentication JWT token validation.
/// </summary>
/// <remarks>
///     This settings class configures the JWT bearer authentication scheme to validate Firebase ID tokens.
///     Used by ASP.NET Core authentication middleware to verify tokens issued by Firebase Authentication.
///     **Configured in appsettings.json under "Authentication:Firebase" section:**
///     <code>
/// {
///   "Authentication": {
///     "Firebase": {
///       "Issuer": "https://securetoken.google.com/your-project-id",
///       "Audience": "your-project-id",
///       "TokenUri": "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com"
///     }
///   }
/// }
/// </code>
///     **JWT Validation Parameters:**
///     - Issuer: Must match Firebase's secure token service URL for your project
///     - Audience: Must match your Firebase project ID
///     - TokenUri: Endpoint for retrieving public keys to verify token signatures
///     **Security Flow:**
///     1. Client authenticates with Firebase and receives ID token (JWT)
///     2. Client sends ID token in Authorization header: "Bearer {token}"
///     3. Middleware validates token signature using Firebase public keys
///     4. Middleware validates issuer, audience, and expiration claims
///     5. User claims are extracted and available via HttpContext.User
///     **Integration:**
///     - Used by JwtBearerOptions in authentication middleware configuration
///     - Tokens are validated on every protected API request
///     - Failed validation returns 401 Unauthorized
///     Replace "your-project-id" with your actual Firebase project ID from Firebase Console.
/// </remarks>
public sealed class FirebaseAuthSettings
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "Authentication:Firebase";

    /// <summary>
    ///     Gets or sets the expected JWT token issuer (Firebase Secure Token Service URL).
    /// </summary>
    /// <remarks>
    ///     Format: https://securetoken.google.com/{your-firebase-project-id}
    ///     Validation fails if token's "iss" claim doesn't match this value.
    /// </remarks>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the expected JWT token audience (Firebase project ID).
    /// </summary>
    /// <remarks>
    ///     Must match your Firebase project ID exactly.
    ///     Validation fails if token's "aud" claim doesn't match this value.
    /// </remarks>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the URL for retrieving Firebase public keys used to verify token signatures.
    /// </summary>
    /// <remarks>
    ///     Default: https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com
    ///     The middleware fetches public keys from this endpoint to verify JWT signatures cryptographically.
    ///     Keys are cached and automatically rotated by Firebase.
    /// </remarks>
    public string TokenUri { get; set; } = string.Empty;
}
