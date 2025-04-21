// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Comments;

namespace ExpertBridge.Api.Core.Entities.Media.CommentMedia;

public class CommentMedia : MediaObject
{
    // Foreign keys
    public string CommentId { get; set; }

    // Navigation properties
    public Comment Comment { get; set; }
}
