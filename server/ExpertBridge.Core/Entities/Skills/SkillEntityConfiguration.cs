using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Skills;

/// <summary>
///     Configures the Entity Framework Core mapping for the <see cref="Skill" /> entity.
/// </summary>
public class SkillEntityConfiguration : IEntityTypeConfiguration<Skill>
{
    /// <summary>
    ///     Configures the entity mapping, relationships, and database constraints for skills.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(SkillEntityConstraints.MaxNameLength);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(SkillEntityConstraints.MaxDescriptionLength);
    }
}
