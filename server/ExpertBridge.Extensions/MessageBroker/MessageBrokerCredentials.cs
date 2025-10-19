namespace ExpertBridge.Extensions.MessageBroker;

/// <summary>
/// Represents RabbitMQ message broker connection credentials for the ExpertBridge application.
/// Defines host URI, username, and password for MassTransit RabbitMQ transport configuration.
/// </summary>
/// <remarks>
/// These credentials are loaded from the "MessageBroker" configuration section and used to establish
/// secure connections to the RabbitMQ message broker for asynchronous message processing.
/// 
/// The message broker enables:
/// - Asynchronous AI processing pipeline (post tagging, user interest embedding, content moderation)
/// - Decoupled architecture between API and background workers
/// - Reliable message delivery with retry and error handling
/// - Scalable message consumption across multiple worker instances
/// 
/// Used by MassTransit to configure RabbitMQ transport with authentication and connection settings.
/// </remarks>
public sealed class MessageBrokerCredentials
{
    /// <summary>
    /// Gets the configuration section name for message broker credentials.
    /// </summary>
    public const string SectionName = "MessageBroker";

    /// <summary>
    /// Gets or sets the RabbitMQ host URI (e.g., "rabbitmq://localhost" or "amqp://production-rabbitmq:5672").
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Gets or sets the RabbitMQ username for authentication.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the RabbitMQ password for authentication.
    /// </summary>
    public required string Password { get; set; }
}
