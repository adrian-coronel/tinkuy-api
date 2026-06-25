namespace StockLinkApi.Models;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Camera> Cameras { get; set; } = [];
    public ICollection<Inventory> Inventory { get; set; } = [];
}
