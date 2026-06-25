namespace StockLinkApi.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Inventory> Inventory { get; set; } = [];
}
