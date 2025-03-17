using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTag;

public class ProfileTagEntityConfiguration : IEntityTypeConfiguration<ProfileTag>
{
    public void Configure(EntityTypeBuilder<ProfileTag> builder)
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
