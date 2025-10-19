// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ProfileExperiences;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="ProfileExperience"/> entity.
/// </summary>
public class ProfileExperienceEntityConfiguration : IEntityTypeConfiguration<ProfileExperience>
{
    /// <summary>
    /// Configures the entity mapping, relationships, and database constraints for profile experiences.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<ProfileExperience> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxTitleLength);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxDescriptionLength);

        builder.Property(x => x.Company)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxCompanyLength);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(ProfileExperienceConstraints.MaxLocationLength);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired(false)
            .ValueGeneratedOnAdd();

        builder.HasOne(x => x.Profile)
            .WithMany(x => x.Experiences)
            .HasForeignKey(x => x.ProfileId)
            .IsRequired();

        builder.HasMany(x => x.Medias)
            .WithOne(x => x.ProfileExperience)
            .HasForeignKey(x => x.ProfileExperienceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false)
            ;
    }
}
