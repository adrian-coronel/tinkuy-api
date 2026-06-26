using Microsoft.EntityFrameworkCore;
using StockLinkApi.Data;
using StockLinkApi.Dtos;
using StockLinkApi.Services;

namespace StockLinkApi.Endpoints;

public static class NotificationEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/notifications", async (Guid userId, bool? unreadOnly, AppDbContext db) =>
        {
            var query = db.Notifications
                .Where(n => n.UserId == userId || n.UserId == null)
                .AsQueryable();

            if (unreadOnly == true)
                query = query.Where(n => !n.IsRead);

            var result = await query
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponse(n.Id, n.UserId, n.Type, n.Title, n.Message, n.Payload, n.IsRead, n.CreatedAt))
                .ToListAsync();

            return Results.Ok(result);
        });

        app.MapPatch("/api/notifications/{id:guid}/read", async (Guid id, AppDbContext db) =>
        {
            var notification = await db.Notifications.FindAsync(id);
            if (notification is null) return Results.NotFound();

            notification.IsRead = true;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapPatch("/api/notifications/read-all", async (Guid userId, AppDbContext db) =>
        {
            await db.Notifications
                .Where(n => (n.UserId == userId || n.UserId == null) && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            return Results.NoContent();
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapPost("/api/notifications/test", async (TestNotificationRequest req, INotificationService svc) =>
            {
                if (req.UserId.HasValue)
                    await svc.SendToUserAsync(req.UserId.Value, req.Type, req.Title, req.Message);
                else
                    await svc.BroadcastAsync(req.Type, req.Title, req.Message);

                return Results.Ok(new { sent = true });
            });
        }
    }
}
