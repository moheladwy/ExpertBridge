// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.NotificationsCleaning;

internal sealed class NotificationsCleaningPeriodicWorkerSetup : IConfigureOptions<QuartzOptions>
{
    /// <summary>
    ///     The group name for the Quartz job and trigger.
    /// </summary>
    private const string Group = "periodic";

    /// <summary>
    ///     The name of the Quartz job for the Notifications Cleaning periodic worker.
    /// </summary>
    private const string JobName = $"{nameof(NotificationsCleaningPeriodicWorker)}.Job";

    /// <summary>
    ///     The name of the Quartz trigger for the Notifications Cleaning periodic worker.
    /// </summary>
    private const string TriggerName = $"{nameof(NotificationsCleaningPeriodicWorker)}.trigger";

    /// <summary>
    ///     The description of the Quartz job for the Notifications Cleaning periodic worker.
    /// </summary>
    private const string Description = "Notifications Cleaning Periodic Worker";

    /// <summary>
    ///     The description of the Quartz trigger for the Notifications Cleaning periodic worker.
    /// </summary>
    private const string TriggerDescription = "Trigger for the Notifications Cleaning Periodic Worker";

    /// <summary>
    ///     The time interval, in hours, for triggering the Notifications Cleaning periodic worker job.
    /// </summary>
    private const int TriggerJobIntervalInHours = 36;

    /// <summary>
    ///     Configures Quartz options to schedule the Notifications Cleaning periodic worker.
    /// </summary>
    /// <param name="options">The Quartz options to configure.</param>
    public void Configure(QuartzOptions options)
    {
        options
            .AddJob<NotificationsCleaningPeriodicWorker>(jobBuilder =>
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
                // to run every 36 hours indefinitely.
                triggerBuilder.ForJob(JobName, Group);
                triggerBuilder.WithDescription(TriggerDescription);
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
