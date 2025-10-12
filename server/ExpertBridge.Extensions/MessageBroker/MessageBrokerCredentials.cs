namespace ExpertBridge.Extensions.MessageBroker;

public sealed class MessageBrokerCredentials
{
    public const string SectionName = "MessageBroker";
    
    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}