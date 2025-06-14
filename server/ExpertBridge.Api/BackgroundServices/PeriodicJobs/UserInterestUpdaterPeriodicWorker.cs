// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public class UserInterestUpdaterPeriodicWorker : PeriodicWorker<UserInterestUpdaterPeriodicWorker>
    {
        private readonly IServiceProvider _services;
        private readonly ChannelWriter<UserInterestsUpdatedMessage> _channel;
        private readonly ILogger<UserInterestUpdaterPeriodicWorker> _logger;

        public UserInterestUpdaterPeriodicWorker(
            IServiceProvider services,
            Channel<UserInterestsUpdatedMessage> channel,
            ILogger<UserInterestUpdaterPeriodicWorker> logger)
            : base(
                PeriodicJobsStartDelays.UserInterestUpdaterPeriodicWorkerStartDelay,
                nameof(UserInterestUpdaterPeriodicWorker),
                logger)
        {
            _services = services;
            _channel = channel.Writer;
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
        {
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

        }
    }
}
