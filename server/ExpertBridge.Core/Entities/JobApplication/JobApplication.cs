using System;
using ExpertBridge.Core.Entities.Profiles;
namespace ExpertBridge.Core.Entities.JobApplication;

public class JobApplication
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string JobPostingId { get; set; }
    public string ContractorProfileId { get; set; }
    public string CoverLetter { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public Profile ContractorProfile { get; set; }

}
