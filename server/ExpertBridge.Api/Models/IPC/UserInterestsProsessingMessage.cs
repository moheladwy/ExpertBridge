namespace ExpertBridge.Api.Models.IPC;

public class UserInterestsProsessingMessage
{
    public string UserProfileId { get; set; }

    public List<string> Interests { get; set; }
}
