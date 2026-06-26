using Microsoft.AspNetCore.SignalR;
using StockLinkApi.Data;
using StockLinkApi.Hubs;
using StockLinkApi.Models;
using System.Text.Json;

namespace StockLinkApi.Services;

public class NotificationService(AppDbContext db, IHubContext<NotificationHub> hub) : INotificationService
{
    public async Task SendToUserAsync(Guid userId, string type, string title, string message, object? payload = null)
    {
        var notification = await PersistAsync(userId, type, title, message, payload);
        await hub.Clients.Group(userId.ToString()).SendAsync("notification", ToEnvelope(notification));
    }

    public async Task BroadcastAsync(string type, string title, string message, object? payload = null)
    {
        var notification = await PersistAsync(null, type, title, message, payload);
        await hub.Clients.All.SendAsync("notification", ToEnvelope(notification));
    }

    private async Task<Notification> PersistAsync(Guid? userId, string type, string title, string message, object? payload)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Payload = payload is null ? null : JsonSerializer.Serialize(payload),
            IsRead = false
        };
        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
        return notification;
    }

    private static object ToEnvelope(Notification n) => new
    {
        n.Id,
        n.UserId,
        n.Type,
        n.Title,
        n.Message,
        n.Payload,
        n.IsRead,
        n.CreatedAt
    };
}
