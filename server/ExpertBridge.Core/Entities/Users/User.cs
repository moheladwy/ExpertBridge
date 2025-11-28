// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Users;

/// <summary>
///     Represents a user account in the ExpertBridge platform.
/// </summary>
/// <remarks>
///     The User entity stores authentication and account information, with a one-to-one relationship to the
///     <see cref="Profile" /> entity
///     which contains the user's professional information and content.
/// </remarks>
public sealed class User : BaseModel, ISoftDeletable
{
    /// <summary>
    ///     Gets or sets the unique provider identifier from Firebase Authentication.
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
    ///     Gets or sets the unique username for the user.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    ///     Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user is banned from the platform.
    /// </summary>
    public bool IsBanned { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user's email address has been verified.
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has completed the onboarding process.
    /// </summary>
    public bool IsOnboarded { get; set; }

    /// <summary>
    ///     Gets or sets the user's profile containing professional information and content.
    /// </summary>
    /// <remarks>
    ///     This navigation property establishes a one-to-one relationship with the <see cref="Profile" /> entity.
    /// </remarks>
    [JsonIgnore]
    public Profile Profile { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user account is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the user account was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
