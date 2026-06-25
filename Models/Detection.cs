namespace StockLinkApi.Models;

public class Detection
{
    public Guid Id { get; set; }
    public Guid CameraId { get; set; }
    public string? RawImageUrl { get; set; }
    public List<DetectedProductItem> DetectedProducts { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    public Camera Camera { get; set; } = null!;
}

public class DetectedProductItem
{
    public string Producto { get; set; } = string.Empty;
    public int CantidadEstimada { get; set; }
}
