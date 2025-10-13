using System.Net.Http.Json;
using ExpertBridge.Application.Settings;
using FirebaseAdmin.Auth;

namespace ExpertBridge.Application.Services;

/// <summary>
///     Provides authentication services implementing Firebase functionality.
/// </summary>
/// <remarks>
///     Contains methods for user registration, login, and token verification using Firebase Authentication.
/// </remarks>
public class FirebaseAuthService
{
    /// <summary>
    ///     Represents the Firebase Authentication instance used for managing user authentication operations.
    /// </summary>
    /// <remarks>
    ///     This variable holds a default instance of Firebase Authentication and is used to perform operations
    ///     such as creating new users, verifying ID tokens, and handling other authentication-related tasks in the Firebase
    ///     context.
    /// </remarks>
    private readonly FirebaseAuth _auth = FirebaseAuth.DefaultInstance;

    /// <summary>
    ///     Represents the HTTP client instance used for sending requests to the Firebase Authentication service.
    /// </summary>
    /// <remarks>
    ///     This variable is configured with a base address pointing to the authentication token endpoint specified in the
    ///     Firebase settings.
    ///     It is used to perform HTTP operations such as posting authentication requests and handling Firebase token
    ///     transactions.
    /// </remarks>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Service implementation for Firebase authentication, providing functionality for user registration, login, and token
    ///     validation.
    /// </summary>
    /// <remarks>
    ///     This class leverages Firebase Authentication for securely managing user credentials and session tokens.
    ///     It provides methods to register new users, authenticate existing users with email/password credentials,
    ///     and verify Firebase ID tokens to ensure the validity of user sessions.
    /// </remarks>
    /// <param name="httpClient">
    ///     The HTTP client used to communicate with Firebase Authentication endpoints.
    /// </param>
    public FirebaseAuthService(HttpClient httpClient) => _httpClient = httpClient;

    /// <summary>
    ///     Asynchronously registers a new user with Firebase Authentication using the provided email and password.
    /// </summary>
    /// <param name="email">
    ///     The email address of the user to register. Must be unique and valid.
    /// </param>
    /// <param name="password">
    ///     The password for the user. Must meet Firebase password complexity requirements.
    /// </param>
    /// <returns>
    ///     A unique identifier (UID) for the newly registered user.
    /// </returns>
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
    ///     Authenticates a user by email and password using Firebase Authentication and returns a Firebase ID token.
    /// </summary>
    /// <param name="email">
    ///     The email address of the user attempting to log in.
    /// </param>
    /// <param name="password">
    ///     The password associated with the provided email address.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the Firebase ID token
    ///     for the authenticated user upon a successful login.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the response from Firebase Authentication is not as expected or the authentication token cannot be
    ///     parsed.
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
    ///     Verifies the validity of a Firebase ID token and decodes it into a FirebaseToken object.
    /// </summary>
    /// <remarks>
    ///     This method is used for validating Firebase ID tokens to verify user sessions.
    ///     It ensures that the token is authentic and decodes the token to extract the user's information.
    /// </remarks>
    /// <param name="idToken">
    ///     The Firebase ID token to be verified.
    /// </param>
    /// <returns>
    ///     A <see cref="FirebaseToken" /> object containing the decoded information from the ID token if verification is
    ///     successful;
    ///     otherwise, returns null if the token is invalid or verification fails.
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
