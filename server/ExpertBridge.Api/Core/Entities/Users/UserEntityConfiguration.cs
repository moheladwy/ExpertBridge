// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.Users;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ProviderId)
            .IsRequired()
            .HasMaxLength(GlobalEntitiesConstraints.MaxIdLength);

        builder.HasIndex(x => x.ProviderId).IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.MaxEmailLength);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.Username)
            .IsRequired(false)
            .HasMaxLength(UserEntityConstraints.MaxUsernameLength);

        builder.HasIndex(x => x.Username).IsUnique();

        builder.Property(x => x.PhoneNumber)
            .IsRequired(false)
            .HasMaxLength(UserEntityConstraints.MaxPhoneNumberLength);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.MaxNameLength);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.MaxNameLength);

        builder.Property(x => x.IsBanned)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.IsEmailVerified)
            .IsRequired();

        builder.Property(x => x.IsOnboarded)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd();
    }
}
