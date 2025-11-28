namespace ExpertBridge.Admin.ViewModels;

public sealed class PostTagsViewModel
{
    public string TagId { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string ArabicName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public int UserInterestCount { get; set; }
    public int JobPostingCount { get; set; }

    public bool IsUsedInPosts
    {
        get { return PostCount > 0; }
    }

    public bool IsUsedInUserInterests
    {
        get { return UserInterestCount > 0; }
    }

    public bool IsUsedInJobPostings
    {
        get { return JobPostingCount > 0; }
    }

    public bool IsUsedAnywhere
    {
        get { return IsUsedInPosts || IsUsedInUserInterests || IsUsedInJobPostings; }
    }

    public string UsageLocations
    {
        get
        {
            var locations = new List<string>();
            if (IsUsedInPosts)
            {
                locations.Add("Posts");
            }

            if (IsUsedInUserInterests)
            {
                locations.Add("User Interests");
            }

            if (IsUsedInJobPostings)
            {
                locations.Add("Job Postings");
            }

            return locations.Any() ? string.Join(", ", locations) : "Not Used";
        }
    }
}
