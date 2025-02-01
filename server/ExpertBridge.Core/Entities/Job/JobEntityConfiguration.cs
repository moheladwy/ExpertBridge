using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Job;

public class JobEntityConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActualCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.EndedAt)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        // TODO: Add the rest of the properties
    }
}
