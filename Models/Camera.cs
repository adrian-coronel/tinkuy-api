namespace StockLinkApi.Models;

public class Camera
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string Status { get; set; } = "online";
    public DateTime? LastSyncAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Store Store { get; set; } = null!;
    public ICollection<Detection> Detections { get; set; } = [];
}
