// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Media.ProfileMedia;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="ProfileMedia"/> entity.
/// </summary>
public class ProfileMediaEntityConfiguration : IEntityTypeConfiguration<ProfileMedia>
{
    /// <summary>
    /// Configures the entity mapping using shared media object configuration.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<ProfileMedia> builder) => builder.ConfigureAbstractMedia();
}
