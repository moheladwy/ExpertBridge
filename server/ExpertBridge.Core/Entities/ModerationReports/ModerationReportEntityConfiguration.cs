using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ModerationReports
{
    public class ModerationReportEntityConfiguration : IEntityTypeConfiguration<ModerationReport>
    {
        public void Configure(EntityTypeBuilder<ModerationReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ContentType)
                .HasConversion<string>();

            builder.HasIndex(x => x.IsNegative);
            builder.HasIndex(x => x.ContentId);
            builder.HasIndex(x => new { x.IsNegative, x.ContentId });
        }
    }
}
