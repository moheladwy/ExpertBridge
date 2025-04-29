// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Data.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.ProfileExperienceMedia;

public class ProfileExperienceMediaEntityConfiguration : IEntityTypeConfiguration<ProfileExperienceMedia>
{
    public void Configure(EntityTypeBuilder<ProfileExperienceMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
