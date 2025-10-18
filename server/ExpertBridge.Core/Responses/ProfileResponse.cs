// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
/// Represents the response DTO for user profile information.
/// </summary>
/// <remarks>
/// This DTO aggregates professional profile data including ratings, skills, and reputation metrics
/// for display in profile views and search results.
/// </remarks>
public class ProfileResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated user account.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the average rating of the user's work and expertise.
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Gets or sets the total number of ratings received.
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's unique username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is banned from the platform.
    /// </summary>
    public bool IsBanned { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user's current job title or professional designation.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the user's professional biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the URL of the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has completed the onboarding process.
    /// </summary>
    public bool IsOnboarded { get; set; }

    /// <summary>
    /// Gets or sets the total number of upvotes received on the user's comments.
    /// </summary>
    public int CommentsUpvotes { get; set; }

    /// <summary>
    /// Gets or sets the total number of downvotes received on the user's comments.
    /// </summary>
    public int CommentsDownvotes { get; set; }

    /// <summary>
    /// Gets or sets the user's reputation score calculated from engagement metrics.
    /// </summary>
    public int Reputation { get; set; }

    /// <summary>
    /// Gets or sets the list of skill names associated with the profile.
    /// </summary>
    public List<string>? Skills { get; set; } = [];
}
