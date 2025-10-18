// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for user search results.
/// </summary>
/// <remarks>
/// This DTO is used when returning users from search queries, including relevance ranking
/// based on semantic similarity and text matching.
/// </remarks>
public class SearchUserResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

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

    /// <summary>
    /// Gets or sets the user's job title.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the user's professional biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the relevance rank of the search result.
    /// </summary>
    /// <remarks>
    /// Higher ranks indicate better matches to the search query.
    /// This value is calculated using semantic embeddings and text similarity.
    /// </remarks>
    public double Rank { get; set; }
}
