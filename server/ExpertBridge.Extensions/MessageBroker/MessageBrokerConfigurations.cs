using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Extensions.MessageBroker;

/// <summary>
///     Provides extension methods for configuring message broker services and credentials.
/// </summary>
public static class MessageBrokerConfigurations
{
    /// <summary>
    ///     Configures and registers MessageBrokerCredentials as IOptions in the dependency injection container.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method reads the message broker credentials from the configuration section
    ///     specified by <see cref="MessageBrokerCredentials.SectionName" /> and registers them
    ///     as IOptions&lt;MessageBrokerCredentials&gt; for dependency injection.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the required configuration section is missing or cannot be parsed.
    /// </exception>
    private static TBuilder ConfigureMessageBrokerCredentials<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Get the credentials from the configs/secrets files
        var credentials = builder.Configuration
            .GetRequiredSection(MessageBrokerCredentials.SectionName)
            .Get<MessageBrokerCredentials>();

        // Register MessageBrokerCredentials as IOptions for DI
        builder.Services.Configure<MessageBrokerCredentials>(configureOptions =>
        {
            configureOptions.Host = credentials!.Host;
            configureOptions.Username = credentials.Username;
            configureOptions.Password = credentials.Password;
        });

        // return the builder after registering the credentials as an IOptions
        return builder;
    }

    /// <summary>
    ///     Registers and configures MassTransit with RabbitMQ as the message broker.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder instance.</param>
    /// <param name="consumerAssembly">
    ///     Optional consumerAssembly to scan for message consumers. If provided, all consumers
    ///     in the consumerAssembly will be automatically registered. Pass null to skip consumer registration.
    /// </param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    ///     This method performs the following operations:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Configures message broker credentials via
    ///                 <see cref="ConfigureMessageBrokerCredentials{TBuilder}" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>Registers MassTransit with kebab-case endpoint name formatting.</description>
    ///         </item>
    ///         <item>
    ///             <description>Optionally registers all message consumers from the specified consumerAssembly.</description>
    ///         </item>
    ///         <item>
    ///             <description>Configures RabbitMQ as the transport layer with host, username, and password.</description>
    ///         </item>
    ///         <item>
    ///             <description>Automatically configures endpoints based on registered consumers.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <example>
    ///     <code>
    /// // Register without consumers
    /// builder.RegisterMessageBroker();
    /// 
    /// // Register with consumers from current consumerAssembly
    /// builder.RegisterMessageBroker(Assembly.GetExecutingAssembly());
    /// </code>
    /// </example>
    public static TBuilder RegisterMessageBroker<TBuilder>(this TBuilder builder, Assembly? consumerAssembly = null)
        where TBuilder : IHostApplicationBuilder
    {
        // Configure message broker credentials by binding them to IOptions<MessageBrokerCredentials>.
        builder.ConfigureMessageBrokerCredentials();

        // Add and configure MassTransit services for dependency injection.
        builder.Services.AddMassTransit(configure =>
        {
            // Set endpoint names to use kebab-case formatting.
            configure.SetKebabCaseEndpointNameFormatter();

            // If a consumerAssembly is provided, register all consumers from the specified consumerAssembly.
            if (consumerAssembly is not null)
            {
                configure.AddConsumers(consumerAssembly);
            }

            // Configure RabbitMQ as the transport layer for MassTransit.
            configure.UsingRabbitMq((ctx, cfg) =>
            {
                // Retrieve the message broker credentials from the dependency injection container.
                var credentials = ctx.GetRequiredService<IOptions<MessageBrokerCredentials>>().Value;

                // Configure the RabbitMQ host with the retrieved credentials.
                cfg.Host(new Uri(credentials.Host), h =>
                {
                    h.Username(credentials.Username);
                    h.Password(credentials.Password);
                });

                // Automatically configure endpoints for all registered consumers.
                cfg.ConfigureEndpoints(ctx);
                cfg.UseInstrumentation();
                cfg.UseMessageRetry(retryConfigurator => { retryConfigurator.Interval(2, 1000); });
                cfg.ConcurrentMessageLimit = 2;
            });
        });

        // Return the builder instance for method chaining.
        return builder;
    }
}
