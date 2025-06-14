namespace ExpertBridge.Core.Responses
{
    public class JobResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public double ActualCost { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? JobPostingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ProfileSummaryResponse AuthorProfile { get; set; } // client
        public ProfileSummaryResponse WorkerProfile { get; set; } // contractor
    }
}