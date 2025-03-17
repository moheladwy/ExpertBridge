using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Area;

public class AreaEntityConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Governorate)
            .IsRequired()
            .HasMaxLength(AreaEntityConstraints.MaxGovernorateLength)
            .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Governorates>(v)
                );

        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(AreaEntityConstraints.MaxRegionLength);

        builder.HasOne(a => a.Profile)
            .WithMany(p => p.Areas)
            .HasForeignKey(a => a.ProfileId)
            .IsRequired();
    }
}
