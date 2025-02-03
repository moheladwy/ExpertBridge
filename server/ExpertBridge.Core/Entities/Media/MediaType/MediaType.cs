namespace ExpertBridge.Core.Entities.Media.MediaType;

public class MediaType
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTypeEnum Type { get; set; }

    // Navigation properties
    public ICollection<Media> Medias { get; set; } = [];
}
