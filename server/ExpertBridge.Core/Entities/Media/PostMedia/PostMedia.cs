// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using ExpertBridge.Core.Entities.Posts;

namespace ExpertBridge.Core.Entities.Media.PostMedia;

/// <summary>
///     Represents a media attachment associated with a post.
/// </summary>
/// <remarks>
///     Post media files are stored in AWS S3 and can include images, videos, or documents that enrich post content.
/// </remarks>
public class PostMedia : MediaObject
{
    // Foreign keys
    /// <summary>
    ///     Gets or sets the unique identifier of the post this media belongs to.
    /// </summary>
    public string PostId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the post this media is attached to.
    /// </summary>
    [JsonIgnore]
    public Post Post { get; set; }
}
