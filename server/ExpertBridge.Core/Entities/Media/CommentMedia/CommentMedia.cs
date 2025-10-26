// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;

namespace ExpertBridge.Core.Entities.Media.CommentMedia;

/// <summary>
/// Represents a media attachment associated with a comment.
/// </summary>
/// <remarks>
/// Comment media files are stored in AWS S3 and allow users to include visual or document context in their comments.
/// </remarks>
public class CommentMedia : MediaObject
{
    // Foreign keys
    /// <summary>
    /// Gets or sets the unique identifier of the comment this media belongs to.
    /// </summary>
    public string CommentId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the comment this media is attached to.
    /// </summary>
    public Comment Comment { get; set; }
}
