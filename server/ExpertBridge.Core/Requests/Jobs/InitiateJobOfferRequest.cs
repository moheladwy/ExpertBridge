using System.ComponentModel.DataAnnotations;
using ExpertBridge.Core.Entities;

namespace ExpertBridge.Api.Requests.Jobs
{
    public class InitiateJobOfferRequest
    {
        [Required]
        public string ContractorProfileId { get; set; }

        [Required]
        [StringLength(GlobalEntitiesConstraints.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(GlobalEntitiesConstraints.MaxDescriptionLength)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Proposed rate must be greater than 0")]
        public double ProposedRate { get; set; }

        public string? JobPostingId { get; set; }
    }
}