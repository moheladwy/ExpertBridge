// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class PeriodicUserInterestUpdaterWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<UserInterestsUpdatedMessage> _channel;
        private readonly ILogger<PeriodicUserInterestUpdaterWorker> _logger;

        public PeriodicUserInterestUpdaterWorker(
            IServiceProvider services,
            Channel<UserInterestsUpdatedMessage> channel,
            ILogger<PeriodicUserInterestUpdaterWorker> logger
            )
        {
            _services = services;
            _channel = channel.Writer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var period = 60 * 60 * 24 * 1; // 1 day
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"{nameof(PeriodicUserInterestUpdaterWorker)} Started...");

                try
                {
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                    var unEmbeddedProfiles = await dbContext.Profiles
                        .AsNoTracking()
                        .Where(p => p.UserInterestEmbedding == null)
                        .Select(p => p.Id)
                        .ToListAsync(stoppingToken);

                    foreach (var profileId in unEmbeddedProfiles)
                    {
                        await _channel.WriteAsync(new UserInterestsUpdatedMessage
                        {
                            UserProfileId = profileId
                        }, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Failed to execute {nameof(PeriodicUserInterestUpdaterWorker)} with exception message {ex.Message}."
                        );
                }

                _logger.LogInformation($"{nameof(PeriodicUserInterestUpdaterWorker)} Finished.");
            }
        }
    }
}
