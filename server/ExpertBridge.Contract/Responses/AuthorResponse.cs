// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Responses;

/// <summary>
///     Represents the response DTO for author/user summary information.
/// </summary>
/// <remarks>
///     This lightweight DTO is used for displaying author information in posts, comments,
///     job postings, and other content where full profile details are not needed.
/// </remarks>
public sealed record AuthorResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier of the profile.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the associated user account.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Gets or sets the author's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///     Gets or sets the author's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    ///     Gets or sets the author's username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     Gets or sets the author's job title.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the author's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
}
