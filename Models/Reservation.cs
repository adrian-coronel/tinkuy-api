namespace StockLinkApi.Models;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid InventoryId { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Inventory Inventory { get; set; } = null!;
}
