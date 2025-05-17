namespace ExpertBridge.Core.Interfaces
{
    public interface ITimestamped
    {
        DateTime? CreatedAt { get; set; }
        DateTime? LastModified { get; set; }
    }
}
