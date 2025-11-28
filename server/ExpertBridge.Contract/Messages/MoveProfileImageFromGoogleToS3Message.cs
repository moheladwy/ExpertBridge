namespace ExpertBridge.Contract.Messages;

public sealed class MoveProfileImageFromGoogleToS3Message
{
    public required string ProfileId { get; set; }
}
