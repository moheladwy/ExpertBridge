// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using Quartz;

namespace ExpertBridge.Worker.PeriodicJobs.MoveImagesFromGoogleToS3;

public class MoveImagesFromGoogleToS3PeriodicWorkerSetup : IConfigureOptions<QuartzOptions>
{
    /// <summary>
    ///     The group name for the Quartz job and trigger.
    /// </summary>
    private const string Group = "periodic";

    /// <summary>
    ///     The name of the Quartz job for the Move Images From Google To S3 Periodic Worker.
    /// </summary>
    private const string JobName = $"{nameof(MoveImagesFromGoogleToS3PeriodicWorker)}.Job";

    /// <summary>
    ///     The name of the Quartz trigger for the Move Images From Google To S3 Periodic Worker..
    /// </summary>
    private const string TriggerName = $"{nameof(MoveImagesFromGoogleToS3PeriodicWorker)}.trigger";

    /// <summary>
    ///     The description of the Quartz job for the Move Images From Google To S3 Periodic Worker..
    /// </summary>
    private const string JobDescription = "Job for the Move Images From Google To S3 Periodic Worker.";

    /// <summary>
    ///     The description of the Quartz trigger for the Move Images From Google To S3 Periodic Worker..
    /// </summary>
    private const string TriggerDescription = "Trigger for the Move Images From Google To S3 Periodic Worker.";

    /// <summary>
    ///     The time interval, in hours, for triggering the Move Images From Google To S3 Periodic Worker.
    /// </summary>
    private const int TriggerJobIntervalInHours = 10;


    public void Configure(QuartzOptions options)
    {
        options
            .AddJob<MoveImagesFromGoogleToS3PeriodicWorker>(jobBuilder =>
            {
                // Configures the job to be stored durably and assigns it an identity.
                jobBuilder.StoreDurably();
                jobBuilder.WithIdentity(JobName, Group);
                jobBuilder.WithDescription(JobDescription);
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
