// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Data.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Media.ChatMedia;

public class ChatMediaEntityConfiguration : IEntityTypeConfiguration<ChatMedia>
{
    public void Configure(EntityTypeBuilder<ChatMedia> builder)
    {
        builder.ConfigureAbstractMedia();
    }
}
