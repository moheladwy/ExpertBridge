namespace ExpertBridge.Api.Models;

public static class PeriodicJobsStartDelays
{
    public const int PostTaggingPeriodicWorkerStartDelay = 1;
    public const int PostEmbeddingPeriodicWorkerStartDelay = 2;
    public const int ContentModerationPeriodicWorkerStartDelay = 4;
    public const int CleanUpNotificationsPeriodicWorkerStartDelay = 8;
    public const int S3CleaningPeriodicWorkerStartDelay = 12;
    public const int UserInterestUpdaterPeriodicWorkerStartDelay = 16;
}
