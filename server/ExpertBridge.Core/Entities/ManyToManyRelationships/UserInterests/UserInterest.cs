// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Tags;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;

/// <summary>
///     Represents a many-to-many relationship between profiles and tags representing user interests.
/// </summary>
/// <remarks>
///     User interests are tags that indicate topics and categories users care about. These interests drive
///     personalized content recommendations using vector embeddings (1024-dimensional via Ollama) for semantic similarity
///     matching. Interests can be selected during onboarding or updated based on user interactions with posts.
/// </remarks>
public sealed class UserInterest
{
    /// <summary>
    ///     Gets or sets the unique identifier of the profile.
    /// </summary>
    public string ProfileId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the tag representing the interest.
    /// </summary>
    public string TagId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the profile that has this interest.
    /// </summary>
    public Profile Profile { get; set; }

    /// <summary>
    ///     Gets or sets the tag representing the user's interest.
    /// </summary>
    public Tag Tag { get; set; }
}
