// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities;

/// <summary>
///     Defines the types of content that can exist in the ExpertBridge platform.
/// </summary>
/// <remarks>
///     Used to categorize different content types for media attachments, moderation reports, and type-specific processing.
/// </remarks>
public enum ContentTypes
{
    /// <summary>
    ///     A user-created post in the social feed.
    /// </summary>
    Post,

    /// <summary>
    ///     A comment on a post or job posting.
    /// </summary>
    Comment,

    /// <summary>
    ///     A job posting created by a hirer seeking workers.
    /// </summary>
    JobPosting,

    /// <summary>
    ///     A user's professional profile.
    /// </summary>
    Profile,

    /// <summary>
    ///     A chat message between users.
    /// </summary>
    Message,

    /// <summary>
    ///     Video content attachment.
    /// </summary>
    Video,

    /// <summary>
    ///     Image content attachment.
    /// </summary>
    Image,

    /// <summary>
    ///     Generic file attachment.
    /// </summary>
    File
}
