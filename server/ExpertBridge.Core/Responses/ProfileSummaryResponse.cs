// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for minimal profile summary information.
/// </summary>
/// <remarks>
/// This lightweight DTO is used in contexts where only basic profile identification
/// and display information is needed, such as in lists or compact views.
/// </remarks>
public class ProfileSummaryResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public string ProfileId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
}
