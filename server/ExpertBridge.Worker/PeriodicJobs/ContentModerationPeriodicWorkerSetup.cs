// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs;

/// <summary>
/// Configures the Quartz scheduler options for the Content Moderation periodic worker job.
/// </summary>
internal sealed class ContentModerationPeriodicWorkerSetup : IConfigureOptions<QuartzOptions>
{
    /// <summary>
    /// The group name for the Quartz job and trigger.
    /// </summary>
    private const string Group = "scheduler";

    /// <summary>
    /// The name of the Quartz job for the Content Moderation periodic worker.
    /// </summary>
    private const string JobName = nameof(ContentModerationPeriodicWorker);

    /// <summary>
    /// The name of the Quartz trigger for the Content Moderation periodic worker.
    /// </summary>
    private const string TriggerName = $"{JobName}.trigger";

    /// <summary>
    /// The description of the Quartz job for the Content Moderation periodic worker.
    /// </summary>
    private const string Description = "Content Moderation Periodic Worker";

    /// <summary>
    /// The time interval, in hours, for triggering the Content Moderation periodic worker job.
    /// </summary>
    private const int TriggerJobIntervalInHours = 24;

    /// <summary>
    /// Configures the Quartz options to add the Content Moderation periodic worker job and its trigger.
    /// </summary>
    /// <param name="options">The Quartz options to configure.</param>
    public void Configure(QuartzOptions options)
    {
        options
            .AddJob<ContentModerationPeriodicWorker>(jobBuilder =>
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
}
