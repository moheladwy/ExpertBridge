// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.UpdateProfileRequest;

/// <summary>
/// Represents a request to update user profile information.
/// </summary>
/// <remarks>
/// All properties are optional. Only provided properties will be updated.
/// Skills are replaced entirely if provided.
/// </remarks>
public class UpdateProfileRequest
{
    /// <summary>
    /// Gets or sets the user's job title or professional designation.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the user's professional biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    /// <remarks>
    /// Usernames must be unique across the platform.
    /// </remarks>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the collection of skill IDs to associate with the profile.
    /// </summary>
    /// <remarks>
    /// This replaces all existing skills. Pass an empty list to remove all skills.
    /// </remarks>
    public List<string> Skills { get; set; } = [];
}
