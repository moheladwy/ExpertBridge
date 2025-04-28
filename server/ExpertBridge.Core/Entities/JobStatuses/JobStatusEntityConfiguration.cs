

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.JobStatuses;

public class JobStatusEntityConfiguration : IEntityTypeConfiguration<JobStatus>
{
    public void Configure(EntityTypeBuilder<JobStatus> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<JobStatusEnum>(v)
            )
            .HasMaxLength(GlobalEntitiesConstraints.MaxEnumsLength);
    }
}
