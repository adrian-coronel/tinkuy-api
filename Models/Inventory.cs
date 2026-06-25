namespace StockLinkApi.Models;

public class Inventory
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public int StockQuantity { get; set; }
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Store Store { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = [];
}
