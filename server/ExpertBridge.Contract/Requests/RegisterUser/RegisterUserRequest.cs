// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Requests.RegisterUser;

/// <summary>
///     Represents a request to register a new user account.
/// </summary>
/// <remarks>
///     This request is typically called after Firebase authentication succeeds.
///     The ProviderId should match the Firebase user ID.
/// </remarks>
public class RegisterUserRequest
{
    /// <summary>
    ///     Gets or sets the Firebase authentication provider ID.
    /// </summary>
    /// <remarks>
    ///     This should be the unique Firebase user ID (UID) from Firebase Authentication.
    /// </remarks>
    public string ProviderId { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     Gets or sets the user's desired username.
    /// </summary>
    /// <remarks>
    ///     Usernames must be unique across the platform.
    /// </remarks>
    public string Username { get; set; }

    /// <summary>
    ///     Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///     Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; }
}
