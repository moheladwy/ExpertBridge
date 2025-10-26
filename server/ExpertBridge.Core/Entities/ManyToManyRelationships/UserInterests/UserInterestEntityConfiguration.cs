// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="UserInterest" /> entity.
/// </summary>
public class UserInterestEntityConfiguration : IEntityTypeConfiguration<UserInterest>
{
    /// <summary>
    ///     Configures the entity mapping, composite key, and relationships for user interests.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        // Configure composite key
        builder.HasKey(pt => new { pt.ProfileId, pt.TagId });

        // Configure relationship with Profile
        builder.HasOne(pt => pt.Profile)
            .WithMany(p => p.ProfileTags)
            .HasForeignKey(pt => pt.ProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Tag
        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.ProfileTags)
            .HasForeignKey(pt => pt.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
