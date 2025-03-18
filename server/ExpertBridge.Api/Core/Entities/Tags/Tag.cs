// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTags;

namespace ExpertBridge.Api.Core.Entities.Tags;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ProfileTag> ProfileTags { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
}
