using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Skill;

public class SkillEntityConfiguration : IEntityTypeConfiguration<Skill>
{
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
            .IsRequired()
            .HasMaxLength(SkillEntityConstraints.MaxDescriptionLength);
    }
}
