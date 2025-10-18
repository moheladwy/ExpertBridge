// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.UserInterestUpdater;

internal sealed class UserInterestUpdaterPeriodicWorkerSetup : IConfigureOptions<QuartzOptions>
{
    /// <summary>
    ///     The group name for the Quartz job and trigger.
    /// </summary>
    private const string Group = "periodic";

    /// <summary>
    ///     The name of the Quartz job.
    /// </summary>
    private const string JobName = $"{nameof(UserInterestUpdaterPeriodicWorker)}.Job";

    /// <summary>
    ///     The name of the Quartz trigger.
    /// </summary>
    private const string TriggerName = $"{nameof(UserInterestUpdaterPeriodicWorker)}.trigger";

    /// <summary>
    ///     The description of the Quartz job.
    /// </summary>
    private const string Description = "User Interests Embedding Periodic Worker";

    /// <summary>
    ///     The time interval, in hours, for triggering.
    /// </summary>
    private const int TriggerJobIntervalInHours = 24;

    public void Configure(QuartzOptions options) =>
        options
            .AddJob<UserInterestUpdaterPeriodicWorker>(jobBuilder =>
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
                triggerBuilder.ForJob(JobName, Group);
                triggerBuilder.WithSimpleSchedule(scheduleBuilder =>
                {
                    scheduleBuilder
                        .WithIntervalInHours(TriggerJobIntervalInHours)
                        .RepeatForever();
                });
                triggerBuilder.WithIdentity(TriggerName, Group);
            });
}
