// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ProfileExperiences;

namespace ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMedia : MediaObject
{

    // Foreign keys
    public string ProfileExperienceId { get; set; }

    // Navigation properties
    public ProfileExperience ProfileExperience { get; set; }
}
