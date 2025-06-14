// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.CreateComment;

public class CreateCommentRequest
{
    public required string PostId { get; set; }
    public string? ParentCommentId { get; set; }
    public required string Content { get; set; }
    public List<MediaObjectRequest>? Media { get; set; }
}
