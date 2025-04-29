// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTags;

public class ProfileTag
{
    public string ProfileId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public Tags.Tag Tag { get; set; }
}
