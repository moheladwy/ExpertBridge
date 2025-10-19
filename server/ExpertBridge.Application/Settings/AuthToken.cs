using System.Text.Json.Serialization;

namespace ExpertBridge.Application.Settings;

/// <summary>
/// Internal DTO for deserializing Firebase Authentication token responses.
/// Maps JSON responses from Firebase Auth REST API to strongly-typed properties.
/// </summary>
/// <remarks>
/// This class is used internally by Firebase authentication services to parse token responses
/// when users sign in or refresh their authentication tokens.
/// 
/// **Firebase Auth API Response Example:**
/// <code>
/// {
///   "kind": "identitytoolkit#VerifyPasswordResponse",
///   "localId": "user123",
///   "email": "user@example.com",
///   "displayName": "John Doe",
///   "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6...",
///   "registered": true,
///   "refreshToken": "AEu4IL2...",
///   "expiresIn": "3600"
/// }
/// </code>
/// 
/// Properties are decorated with JsonPropertyName attributes to match Firebase's camelCase JSON format
/// while maintaining C# PascalCase naming conventions.
/// 
/// Not exposed publicly - used only within authentication service implementations.
/// </remarks>
internal sealed class AuthTokenSettings
{
    /// <summary>
    /// Gets or sets the kind of response from Firebase (e.g., "identitytoolkit#VerifyPasswordResponse").
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    /// <summary>
    /// Gets or sets the unique user identifier in Firebase Authentication.
    /// </summary>
    [JsonPropertyName("localId")]
    public string LocalId { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the Firebase ID token (JWT) used for authenticating API requests.
    /// </summary>
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is already registered in Firebase.
    /// </summary>
    [JsonPropertyName("registered")]
    public bool Registered { get; set; }

    /// <summary>
    /// Gets or sets the refresh token used to obtain new ID tokens when they expire.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the ID token expiration time in seconds (typically "3600" for 1 hour).
    /// </summary>
    [JsonPropertyName("expiresIn")]
    public string ExpiresIn { get; set; }
}
