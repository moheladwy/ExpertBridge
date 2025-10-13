// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.JobOffers;

public class JobOffer : BaseModel, ISoftDeletable
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal Budget { get; set; }
    public required string Area { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsDeclined { get; set; }

    // Foreign keys
    public string AuthorId { get; set; }
    public string WorkerId { get; set; }

    // Navigation properties
    public Profile Author { get; set; }
    public Profile Worker { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
