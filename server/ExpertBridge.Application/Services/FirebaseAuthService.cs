using System.Net.Http.Json;
using ExpertBridge.Application.Settings;
using FirebaseAdmin.Auth;

namespace ExpertBridge.Application.Services;

/// <summary>
///     Provides comprehensive authentication services using Firebase Authentication for user management and session
///     validation.
/// </summary>
/// <remarks>
///     This service acts as a wrapper around Firebase Admin SDK and Firebase Authentication REST API,
///     providing a unified interface for authentication operations in the ExpertBridge platform.
///     HttpClient should be configured with the Firebase Authentication endpoint in dependency injection.
/// </remarks>
public sealed class FirebaseAuthService
{
    /// <summary>
    ///     The Firebase Admin SDK authentication instance for server-side user management operations.
    /// </summary>
    /// <remarks>
    ///     Uses the default Firebase app instance configured during application startup.
    ///     Provides administrative access to Firebase Authentication including user creation,
    ///     token verification, and custom claims management.
    /// </remarks>
    private readonly FirebaseAuth _auth = FirebaseAuth.DefaultInstance;

    /// <summary>
    ///     The HTTP client configured for Firebase Authentication REST API endpoints.
    /// </summary>
    /// <remarks>
    ///     Should be configured with BaseAddress pointing to Firebase Authentication token endpoint:
    ///     https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={API_KEY}
    ///     Used for password-based authentication which is not available in Admin SDK.
    /// </remarks>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FirebaseAuthService" /> class with the configured HTTP client.
    /// </summary>
    /// <param name="httpClient">
    ///     The HTTP client for communicating with Firebase Authentication REST API.
    ///     Should be pre-configured with the authentication token endpoint URL.
    /// </param>
    /// <remarks>
    ///     HttpClient should be registered in DI with a named or typed client configuration
    ///     that includes the Firebase Authentication endpoint as BaseAddress.
    ///     Example DI registration:
    ///     <code>
    ///         services.AddHttpClient&lt;FirebaseAuthService&gt;(client =>
    ///         {
    ///             client.BaseAddress = new Uri(firebaseSettings.AuthenticationTokenUri);
    ///         });
    ///     </code>
    /// </remarks>
    public FirebaseAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Asynchronously registers a new user in Firebase Authentication and returns the unique user identifier.
    /// </summary>
    /// <param name="email">
    ///     The email address for the new user account. Must be unique and valid according to Firebase rules.
    /// </param>
    /// <param name="password">
    ///     The password for the new user account. Must meet Firebase password requirements (minimum 6 characters).
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the Firebase UID (unique identifier) of the newly
    ///     created user.
    /// </returns>
    /// <remarks>
    ///     This method uses Firebase Admin SDK to create a new user.
    ///     After registration, the client should call LoginAsync to get an ID token for API authentication.
    /// </remarks>
    /// <exception cref="FirebaseAuthException">
    ///     Thrown when user creation fails due to duplicate email, invalid credentials, or Firebase service errors.
    /// </exception>
    public async Task<string> RegisterAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
            Disabled = false,
            DisplayName = email,
            EmailVerified = true
        };

        var userRecord = await _auth.CreateUserAsync(userArgs);
        return userRecord.Uid;
    }

    /// <summary>
    ///     Authenticates a user with email and password, returning a Firebase ID token (JWT) for API access.
    /// </summary>
    /// <param name="email">
    ///     The email address of the user attempting to authenticate.
    /// </param>
    /// <param name="password">
    ///     The password associated with the email address.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the Firebase ID token (JWT) for the authenticated user.
    ///     This token should be sent in the Authorization header for subsequent API requests.
    /// </returns>
    /// <remarks>
    ///     This method uses Firebase Authentication REST API (not Admin SDK) because password-based sign-in
    ///     requires client-side authentication flow.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    ///     Thrown when the HTTP request to Firebase fails (network error, service unavailable).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the response cannot be deserialized or is in an unexpected format.
    ///     Common causes: malformed JSON response, API version mismatch.
    /// </exception>
    /// <exception cref="FirebaseAuthException">
    ///     Thrown by Firebase for authentication failures: invalid email, wrong password, user disabled, too many attempts.
    ///     Check the error code in the response for specific failure reason.
    /// </exception>
    public async Task<string> LoginAsync(string email, string password)
    {
        var request = new { email, password, returnSecureToken = true };
        var response = await _httpClient.PostAsJsonAsync("", request);
        response.EnsureSuccessStatusCode();

        var authToken = await response.Content.ReadFromJsonAsync<AuthTokenSettings>()
                        ?? throw new InvalidOperationException("Failed to parse authentication token");

        return authToken.IdToken;
    }

    /// <summary>
    ///     Verifies the authenticity and validity of a Firebase ID token and returns the decoded token information.
    /// </summary>
    /// <param name="idToken">
    ///     The Firebase ID token (JWT) to verify. Typically extracted from the Authorization header.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the decoded <see cref="FirebaseToken" /> if valid,
    ///     or null if the token is invalid, expired, or verification fails.
    /// </returns>
    /// <remarks>
    ///     This method is used by authentication middleware to validate tokens on protected API endpoints.
    /// </remarks>
    /// <returns>
    ///     The decoded FirebaseToken containing user claims if verification succeeds, otherwise null.
    ///     Null indicates the token should be rejected and the user should re-authenticate.
    /// </returns>
    public async Task<FirebaseToken?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            return await _auth.VerifyIdTokenAsync(idToken, true);
        }
        catch
        {
            return null; // Invalid session
        }
    }
}
