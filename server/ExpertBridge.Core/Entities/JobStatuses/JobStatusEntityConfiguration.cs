// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Api.Core.Entities.JobStatuses;

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
