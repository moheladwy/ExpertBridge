namespace ExpertBridge.Core.Entities.Media;

public class Media
{
    public string Id { get; set; }

    public string Name { get; set; }

    public MediaType MediaType { get; set; }

    public string MediaUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
