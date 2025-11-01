using ExpertBridge.Worker.PeriodicJobs.CommentsModeration;
using ExpertBridge.Worker.PeriodicJobs.JobPostsModeration;
using ExpertBridge.Worker.PeriodicJobs.MoveImagesFromGoogleToS3;
using ExpertBridge.Worker.PeriodicJobs.NotificationsCleaning;
using ExpertBridge.Worker.PeriodicJobs.PostModeration;
using ExpertBridge.Worker.PeriodicJobs.S3Cleaning;
using ExpertBridge.Worker.PeriodicJobs.UserInterestUpdater;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace ExpertBridge.Worker;

/// <summary>
///     Provides extension methods to configure background services using Quartz scheduler.
/// </summary>
/// <remarks>
///     This class configures Quartz for scheduling background jobs, including configuration
///     of dependency injection, job execution options, and specific job setups.
/// </remarks>
internal static class Extensions
{
    /// <summary>
    ///     Adds and configures Quartz background services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add background services to.</param>
    /// <param name="configuration">The application configuration for retrieving connection strings.</param>
    /// <remarks>
    ///     This method:
    ///     <list type="bullet">
    ///         <item>Configures Quartz to use Microsoft Dependency Injection</item>
    ///         <item>Configures Quartz to use PostgreSQL for persistent job storage</item>
    ///         <item>Sets up Quartz as a hosted service with specific execution options</item>
    ///         <item>
    ///             Registers all periodic worker job configurations
    ///             (S3 cleaning, post moderation, comments moderation,
    ///             job posts moderation, user interest updater,
    ///             notifications cleaning, and image migration)
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the service parameter is null.</exception>
    public static void AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            options.UsePersistentStore(persistentOptions =>
            {
                persistentOptions.UsePostgres(cfg =>
                {
                    cfg.UseDriverDelegate<PostgreSQLDelegate>();
                    cfg.ConnectionString = configuration.GetConnectionString("QuartzDatabase")!;
                    cfg.TablePrefix = "quartz.qrtz_";
                });
                persistentOptions.UseProperties = true;
                persistentOptions.UseNewtonsoftJsonSerializer();
            });
        });

        services.AddQuartzHostedService(options =>
        {
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
            options.StartDelay = TimeSpan.FromSeconds(30);
        });

        services.ConfigureOptions<S3CleaningPeriodicWorkerSetup>();
        services.ConfigureOptions<PostsModerationPeriodicWorkerSetup>();
        services.ConfigureOptions<JobPostsModerationPeriodicWorkerSetup>();
        services.ConfigureOptions<CommentsModerationPeriodicWorkerSetup>();
        services.ConfigureOptions<UserInterestUpdaterPeriodicWorkerSetup>();
        services.ConfigureOptions<NotificationsCleaningPeriodicWorkerSetup>();
        services.ConfigureOptions<MoveImagesFromGoogleToS3PeriodicWorkerSetup>();
    }
}
