// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class UserInterestUpdaterPeriodicWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<UserInterestsUpdatedMessage> _channel;
        private readonly ILogger<UserInterestUpdaterPeriodicWorker> _logger;

        public UserInterestUpdaterPeriodicWorker(
            IServiceProvider services,
            Channel<UserInterestsUpdatedMessage> channel,
            ILogger<UserInterestUpdaterPeriodicWorker> logger
            )
        {
            _services = services;
            _channel = channel.Writer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This delay to break the synchronization with the start of each Priodic Worker's period.
            await Task.Delay(TimeSpan.FromHours(16), stoppingToken);

            var period = 60 * 60 * 24 * 1; // 1 day
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                // _logger.LogInformation($"{nameof(UserInterestUpdaterPeriodicWorker)} Started...");
                Log.Information("{WorkerName} Started...", nameof(UserInterestUpdaterPeriodicWorker));

                try
                {
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

                    // That might look like a weird design decision.
                    // But look, the GROQ API we are using is limited, and thus
                    // we need to make sure that we are not sending too many requests
                    // The queing nature of the PostCreatedWorker allows us to send the requests
                    // one by one ensuring that we are not exceeding the rate limit.

                    await dbContext.Profiles
                        .AsNoTracking()
                        .Where(p => p.UserInterestEmbedding == null)
                        .Select(p => new UserInterestsUpdatedMessage
                        {
                            UserProfileId = p.Id
                        }
                        )
                        .ForEachAsync(async message =>
                            await _channel.WriteAsync(message, stoppingToken),
                            stoppingToken
                        );
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex,
                    //     $"Failed to execute {nameof(UserInterestUpdaterPeriodicWorker)} with exception message {ex.Message}."
                    //     );
                    Log.Error(ex,
                        "Failed to execute {WorkerName} with exception message {Message}.",
                        nameof(UserInterestUpdaterPeriodicWorker), ex.Message);
                }

                // _logger.LogInformation($"{nameof(UserInterestUpdaterPeriodicWorker)} Finished.");
                Log.Information("{WorkerName} Finished.", nameof(UserInterestUpdaterPeriodicWorker));
            }
        }
    }
}
