// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Users;

/// <summary>
/// Defines validation constraints for User entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public static class UserEntityConstraints
{
    /// <summary>
    /// Maximum length for email addresses (256 characters).
    /// </summary>
    public const int MaxEmailLength = 256;

    /// <summary>
    /// Maximum length for usernames (256 characters).
    /// </summary>
    public const int MaxUsernameLength = 256;

    /// <summary>
    /// Maximum length for first name and last name fields (256 characters).
    /// </summary>
    public const int MaxNameLength = 256;

    /// <summary>
    /// Maximum length for phone numbers (20 characters).
    /// </summary>
    public const int MaxPhoneNumberLength = 20;
}
