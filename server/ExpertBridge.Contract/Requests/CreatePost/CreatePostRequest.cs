// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Requests.MediaObject;

namespace ExpertBridge.Contract.Requests.CreatePost;

/// <summary>
///     Represents a request to create a new post.
/// </summary>
/// <remarks>
///     After creation, posts are automatically processed through an AI pipeline for tagging,
///     embedding generation, and content moderation.
/// </remarks>
public sealed class CreatePostRequest
{
    /// <summary>
    ///     Gets or sets the title of the post.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the main content body of the post.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    ///     Gets or sets the collection of media attachments.
    /// </summary>
    /// <remarks>
    ///     Media files should be uploaded to S3 via presigned URLs before creating the post.
    ///     Each MediaObjectRequest should contain the S3 key and MIME type.
    /// </remarks>
    public List<MediaObjectRequest>? Media { get; set; }
}
