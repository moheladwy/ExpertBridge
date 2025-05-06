

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Media.MediaGrants
{
    public class MediaGrantEntityConfiguration : IEntityTypeConfiguration<MediaGrant>
    {
        public void Configure(EntityTypeBuilder<MediaGrant> builder)
        {
            builder.HasKey(x => x.Id);

            // Composite index so that we make effective use of the boolean value 'OnHold'.
            builder.HasIndex(x => new { x.OnHold, x.GrantedAt });
        }
    }
}
