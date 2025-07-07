namespace ExpertBridge.Core.Requests;

public class SearchJobPostsRequest
{
    public required string Query { get; set; }

    public int? Limit { get; set; } = 25;

    public string? Area { get; set; }

    public decimal? MinBudget { get; set; }

    public decimal? MaxBudget { get; set; }

    public bool IsRemote { get; set; } = false;
}
