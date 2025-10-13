// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Tags;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;

public class PostTag
{
    public string PostId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Post Post { get; set; }
    public Tag Tag { get; set; }
}
