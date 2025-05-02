using Amazon.S3;
using Amazon.S3.Model;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class S3CleaningPeriodicWorker : BackgroundService
    {
        private readonly ILogger<S3CleaningPeriodicWorker> _logger;
        private readonly IServiceProvider _services;

        public S3CleaningPeriodicWorker(
            ILogger<S3CleaningPeriodicWorker> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This delay to break the synchronization with the start of each Priodic Worker's period.
            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);

            var period = 60 * 60 * 24 * 2; // 2 days;
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("S3 Cleaning Service Started...");

                try
                {
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();
                    var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
                    var awsSettings = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<AwsSettings>>().Value;

                    var onHoldGrants = await dbContext.MediaGrants
                        .Where(g => g.OnHold && g.GrantedAt < DateTime.UtcNow.AddHours(-1))
                        .ToListAsync(stoppingToken);

                    //var keys = await s3Client
                    //    .GetAllObjectKeysAsync(
                    //    awsSettings.BucketName, "", new Dictionary<string, object>());

                    List<string> validKeys = [];
                    List<MediaObject> deletedMedias = [];

                    // PostMedias
                    await dbContext.PostMedias
                        .IgnoreQueryFilters()
                        .Where(m => !m.IsDeleted)
                        .Include(m => m.Post)
                        .ForEachAsync(media =>
                        {
                            if (media.Post is not null && !media.Post.IsDeleted)
                                validKeys.Add(media.Key);
                            else
                                deletedMedias.Add(media);
                        }, stoppingToken);

                    // CommentMedias
                    await dbContext.CommentMedias
                        .IgnoreQueryFilters()
                        .Where(m => !m.IsDeleted)
                        .Include(m => m.Comment)
                        .ForEachAsync(media =>
                        {
                            if (media.Comment is not null && !media.Comment.IsDeleted)
                                validKeys.Add(media.Key);
                            else
                                deletedMedias.Add(media);
                        }, stoppingToken);

                    // ProfileMedias
                    await dbContext.ProfileMedias
                        .IgnoreQueryFilters()
                        .Where(m => !m.IsDeleted)
                        .Include(m => m.Profile)
                        .ForEachAsync(media =>
                        {
                            if (media.Profile is not null && !media.Profile.IsDeleted)
                                validKeys.Add(media.Key);
                            else
                                deletedMedias.Add(media);
                        }, stoppingToken);

                    // ChatMedias
                    await dbContext.ChatMedias
                        .IgnoreQueryFilters()
                        .Where(m => !m.IsDeleted)
                        .Include(m => m.Chat)
                        .ForEachAsync(media =>
                        {
                            if (media.Chat is not null && !media.Chat.IsDeleted)
                                validKeys.Add(media.Key);
                            else
                                deletedMedias.Add(media);
                        }, stoppingToken);

                    // JobPostingMedias
                    await dbContext.JobPostingMedias
                        .IgnoreQueryFilters()
                        .Where(m => !m.IsDeleted)
                        .Include(m => m.JobPosting)
                        .ForEachAsync(media =>
                        {
                            if (media.JobPosting is not null && !media.JobPosting.IsDeleted)
                                validKeys.Add(media.Key);
                            else
                                deletedMedias.Add(media);
                        }, stoppingToken);

                    // ProfileExperienceMedias
                    await dbContext.ProfileExperienceMedias
                        .IgnoreQueryFilters()
                        .Where(m => !m.IsDeleted)
                        .Include(m => m.ProfileExperience)
                        .ForEachAsync(media =>
                        {
                            if (media.ProfileExperience is not null && !media.ProfileExperience.IsDeleted)
                                validKeys.Add(media.Key);
                            else
                                deletedMedias.Add(media);
                        }, stoppingToken);

                    if (deletedMedias.Count + onHoldGrants.Count > 0)
                    {
                        await s3Client.DeleteObjectsAsync(
                            new DeleteObjectsRequest
                            {
                                BucketName = awsSettings.BucketName,
                                Objects =
                                    deletedMedias.Select(m => m.Key)
                                    .Where(k => !validKeys.Contains(k))
                                    .Concat(onHoldGrants.Select(g => g.Key))
                                    .Select(k => new KeyVersion { Key = k })
                                    .ToList()
                            },
                            stoppingToken);

                        // Delete deleted medias from database
                        dbContext.RemoveRange(deletedMedias);
                        dbContext.RemoveRange(onHoldGrants);
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Failed to execute S3 Cleaning Service with exception message {ex.Message}."
                        );
                }

                _logger.LogInformation("S3 Cleaning Service Finished.");
            }
        }
    }
}
