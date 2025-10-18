// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FirebaseAdmin.Auth;

namespace ExpertBridge.Core.Interfaces;

/// <summary>
/// Defines the contract for Firebase Authentication services.
/// </summary>
public interface IFirebaseAuthService
{
    /// <summary>
    /// Registers a new user with Firebase Authentication.
    /// </summary>
    /// <param name="email">The email address of the user to register.</param>
    /// <param name="password">The password for the new user account.</param>
    /// <returns>The unique identifier (UID) of the newly registered user.</returns>
    /// <exception cref="FirebaseAuthException">Thrown when registration fails due to Firebase Authentication errors.</exception>
    Task<string> RegisterAsync(string email, string password);

    /// <summary>
    /// Authenticates a user with Firebase Authentication.
    /// </summary>
    /// <param name="email">The email address of the user attempting to log in.</param>
    /// <param name="password">The password for authentication.</param>
    /// <returns>The Firebase ID token for the authenticated user.</returns>
    /// <exception cref="FirebaseAuthException">Thrown when authentication fails due to invalid credentials or other Firebase Authentication errors.</exception>
    Task<string> LoginAsync(string email, string password);

    /// <summary>
    /// Verifies and decodes a Firebase ID token.
    /// </summary>
    /// <param name="idToken">The Firebase ID token to verify.</param>
    /// <returns>The decoded <see cref="FirebaseToken"/> if verification succeeds; otherwise, null.</returns>
    /// <remarks>
    /// This method validates the token's signature, expiration, and audience claims.
    /// </remarks>
    Task<FirebaseToken?> VerifyIdTokenAsync(string idToken);
}
