using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Area;

public class AreaEntityConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Governorate)
            .IsRequired()
            .HasMaxLength(AreaEntityConstraints.MaxGovernorateLength);

        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(AreaEntityConstraints.MaxRegionLength);
    }
}
