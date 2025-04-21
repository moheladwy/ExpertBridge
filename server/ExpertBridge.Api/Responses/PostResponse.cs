// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Entities.Profiles;

namespace ExpertBridge.Api.Responses;

public record PostResponse
{
    public string Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public AuthorResponse? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int Comments { get; set; }
    public bool IsUpvoted { get; set; }
    public bool IsDownvoted {  get; set; }
    public List<MediaObjectResponse>? Medias { get; set; }
}
