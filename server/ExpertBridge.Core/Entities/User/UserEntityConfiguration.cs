using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.User;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.MaxEmailLength);
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.MaxUsernameLength);
        builder.Property(x => x.isBanned)
            .IsRequired();
        builder.Property(x => x.isDeleted)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();
    }
}
