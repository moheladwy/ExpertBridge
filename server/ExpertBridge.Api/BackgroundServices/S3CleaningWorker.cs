// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Amazon.S3;
using Amazon.S3.Model;
using ExpertBridge.Api.Core.Entities.Media;
using ExpertBridge.Api.Core.Entities.Media.PostMedia;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Services;
using ExpertBridge.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.BackgroundServices
{
    public class S3CleaningWorker : BackgroundService
    {
        private readonly ILogger<S3CleaningWorker> _logger;
        private readonly IServiceProvider _services;

        public S3CleaningWorker(
            ILogger<S3CleaningWorker> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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

                    var onHoldGrants = dbContext.MediaGrants
                        .Where(g => g.OnHold && g.GrantedAt < DateTime.UtcNow.AddHours(-1));

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

                    if (deletedMedias.Count + onHoldGrants.Count() > 1)
                    {
                        await s3Client.DeleteObjectsAsync(
                            new DeleteObjectsRequest
                            {
                                BucketName = awsSettings.BucketName,
                                Objects =
                                    deletedMedias.Select(m => m.Key)
                                    .Concat(onHoldGrants.Select(g => g.Key))
                                    .Where(k => !validKeys.Contains(k))
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
