// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Data.DatabaseContexts;
using Serilog;
using System.Threading.Channels;

namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs
{
    public abstract class PeriodicWorker<TWorker> : BackgroundService
    {
        private readonly double _startDelay;
        private readonly string _workerName;
        private readonly ILogger<TWorker> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDelay">Start delay for the implementor in hours</param>
        /// <param name="logger"></param>
        public PeriodicWorker(
            double startDelay,
            string workerName,
            ILogger<TWorker> logger)
        {
            _startDelay = startDelay;
            _workerName = workerName;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This delay to break the synchronization with the start of each Priodic Worker's period.
            //await Task.Delay(TimeSpan.FromHours(_startDelay), stoppingToken);

            var period = 60;// * 60 * 24 * 1; // 1 day
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(period));

            while (!stoppingToken.IsCancellationRequested
                    && await timer.WaitForNextTickAsync(stoppingToken))
            {
                Log.Information("{WorkerName} Started...", _workerName);

                await ExecuteInternalAsync(stoppingToken);

                // _logger.LogInformation($"{nameof(PeriodicWorker)} Finished.");
                Log.Information("{WorkerName} Finished.", _workerName);
            }
        }

        protected abstract Task ExecuteInternalAsync(CancellationToken stoppingToken);
    }
}
