using Quartz;

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
    /// <remarks>
    ///     This method:
    ///     <list type="bullet">
    ///         <item>Configures Quartz to use Microsoft Dependency Injection</item>
    ///         <item>Sets up Quartz as a hosted service with specific execution options</item>
    ///         <item>Configures the logging background job</item>
    ///     </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the services parameter is null.</exception>
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
#pragma warning disable CS0618 // Type or member is obsolete
            options.UseMicrosoftDependencyInjectionJobFactory();
#pragma warning restore CS0618 // Type or member is obsolete
        });

        services.AddQuartzHostedService(options =>
        {
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
            options.StartDelay = TimeSpan.FromSeconds(5);
        });
    }
}
