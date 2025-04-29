// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.PostTags;

public class PostTag
{
    public string PostId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Posts.Post Post { get; set; }
    public Tags.Tag Tag { get; set; }
}
