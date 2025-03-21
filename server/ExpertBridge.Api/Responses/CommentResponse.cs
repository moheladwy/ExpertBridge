// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Responses;

public record CommentResponse
{
    public required string Id { get; set; }
    public AuthorResponse? Author { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CommentResponse> Replies { get; set; } = [];
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public bool IsUpvoted { get; set; }
    public bool IsDownvoted { get; set; }
}
