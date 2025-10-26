// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Users;

namespace ExpertBridge.Core.Responses;

/// <summary>
///     Represents the response DTO for user account information.
/// </summary>
/// <remarks>
///     This DTO is returned by user-related API endpoints and contains non-sensitive user account details.
/// </remarks>
public class UserResponse
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserResponse" /> class from a <see cref="User" /> entity.
    /// </summary>
    /// <param name="user">The user entity to map from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user" /> is null.</exception>
    public UserResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        Id = user.Id;
        FirebaseId = user.ProviderId;
        Email = user.Email;
        Username = user.Username;
        PhoneNumber = user.PhoneNumber;
        FirstName = user.FirstName;
        LastName = user.LastName;
        IsBanned = user.IsBanned;
        IsEmailVerified = user.IsEmailVerified;
        IsDeleted = user.IsDeleted;
    }

    /// <summary>
    ///     Gets or sets the unique identifier of the user.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase Authentication provider ID.
    /// </summary>
    public string FirebaseId { get; set; }

    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     Gets or sets the user's unique username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///     Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user is banned from the platform.
    /// </summary>
    public bool IsBanned { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user's email has been verified.
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user account is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
