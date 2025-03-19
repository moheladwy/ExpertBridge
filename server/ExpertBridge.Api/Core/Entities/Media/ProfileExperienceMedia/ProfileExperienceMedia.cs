// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string ProfileExperienceId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public ProfileExperiences.ProfileExperience ProfileExperience { get; set; }
    public MediaObject Media { get; set; }
}
