namespace StockLinkApi.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = [];
}
