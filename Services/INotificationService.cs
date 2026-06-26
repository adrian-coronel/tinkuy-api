namespace StockLinkApi.Services;

public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string type, string title, string message, object? payload = null);
    Task BroadcastAsync(string type, string title, string message, object? payload = null);
}
