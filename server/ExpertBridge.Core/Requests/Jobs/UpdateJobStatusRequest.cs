using System;
using System.ComponentModel.DataAnnotations;
using ExpertBridge.Core.Entities.JobStatuses;

namespace ExpertBridge.Core.Requests.Jobs;

public class UpdateJobStatusRequest
{
    [Required]
    public JobStatusEnum NewStatus { get; set; }

}
