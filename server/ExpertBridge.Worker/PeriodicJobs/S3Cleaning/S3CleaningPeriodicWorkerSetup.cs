// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.S3Cleaning;

internal sealed class S3CleaningPeriodicWorkerSetup : IConfigureOptions<QuartzOptions>
{
    /// <summary>
    ///     The group name for the Quartz job and trigger.
    /// </summary>
    private const string Group = "scheduler";

    /// <summary>
    ///     The name of the Quartz job for the Content Moderation periodic worker.
    /// </summary>
    private const string JobName = nameof(S3CleaningPeriodicWorker);

    /// <summary>
    ///     The name of the Quartz trigger for the Content Moderation periodic worker.
    /// </summary>
    private const string TriggerName = $"{JobName}.trigger";

    /// <summary>
    ///     The description of the Quartz job for the Content Moderation periodic worker.
    /// </summary>
    private const string Description = "S3 Bucket Cleaning Periodic Worker";

    /// <summary>
    ///     The time interval, in hours, for triggering the Content Moderation periodic worker job.
    /// </summary>
    private const int TriggerJobIntervalInHours = 24;

    public void Configure(QuartzOptions options) =>
        options
            .AddJob<S3CleaningPeriodicWorker>(jobBuilder =>
            {
                // Configures the job to be stored durably and assigns it an identity.
                jobBuilder.StoreDurably();
                jobBuilder.WithIdentity(JobName, Group);
                jobBuilder.WithDescription(Description);
                jobBuilder.DisallowConcurrentExecution();
            })
            .AddTrigger(triggerBuilder =>
            {
                // Configures the trigger for the job with a simple schedule
                // to run every 24 hours indefinitely.
                triggerBuilder.ForJob(JobName);
                triggerBuilder.WithSimpleSchedule(scheduleBuilder =>
                {
                    scheduleBuilder
                        .WithIntervalInHours(TriggerJobIntervalInHours)
                        .RepeatForever();
                });
                triggerBuilder.WithIdentity(TriggerName, Group);
            });
}
