// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.UpdateUserRequest;

/// <summary>
///     Represents a request to update user account information.
/// </summary>
/// <remarks>
///     This request typically synchronizes user data from Firebase Authentication
///     with the local database. Most properties are optional.
/// </remarks>
public class UpdateUserRequest
{
    /// <summary>
    ///     Gets or sets the Firebase authentication provider ID.
    /// </summary>
    public required string ProviderId { get; set; }

    /// <summary>
    ///     Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    ///     Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    ///     Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user's email has been verified.
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed onboarding.
    /// </summary>
    public bool IsOnboarded { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase authentication token.
    /// </summary>
    /// <remarks>
    ///     This token is used for authentication and authorization.
    /// </remarks>
    public string? Token { get; set; }
}
