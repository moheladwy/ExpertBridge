// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.EditPost;

/// <summary>
///     Represents a request to edit an existing post.
/// </summary>
/// <remarks>
///     All properties are optional. Only provided properties will be updated.
///     After editing, posts are reprocessed by AI services for updated tagging and embeddings.
/// </remarks>
public class EditPostRequest
{
    /// <summary>
    ///     Gets or sets the new title for the post.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Gets or sets the new content for the post.
    /// </summary>
    public string? Content { get; set; }
}
