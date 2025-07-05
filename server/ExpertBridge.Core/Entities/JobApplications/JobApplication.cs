using System;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;
namespace ExpertBridge.Core.Entities.JobApplications;

public class JobApplication : BaseModel, ISoftDeletable
{
    public required string JobPostingId { get; set; }
    public required string ApplicantId { get; set; }
    public string? CoverLetter { get; set; }
    public required decimal OfferedCost { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Profile Applicant { get; set; }
    public JobPosting JobPosting { get; set; }
}
