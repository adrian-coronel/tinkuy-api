using Microsoft.EntityFrameworkCore;
using StockLinkApi.Data;
using StockLinkApi.Dtos;

namespace StockLinkApi.Endpoints;

public static class StoreEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/stores/{storeId:guid}/inventory", async (Guid storeId, AppDbContext db) =>
        {
            var store = await db.Stores
                .Include(s => s.Cameras)
                .FirstOrDefaultAsync(s => s.Id == storeId);

            if (store is null) return Results.NotFound();

            var camera = store.Cameras.OrderByDescending(c => c.LastSyncAt).FirstOrDefault();

            var inventory = await db.Inventory
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId)
                .Select(i => new InventoryItemDto(i.Id, i.Product.Name, i.StockQuantity, i.Price, i.UpdatedAt))
                .ToListAsync();

            return Results.Ok(new StoreInventoryResponse(
                store.Id, store.Name,
                camera?.Status ?? "offline",
                camera?.LastSyncAt,
                inventory));
        });

        app.MapGet("/api/stores/{storeId:guid}/reservations", async (Guid storeId, AppDbContext db) =>
        {
            var exists = await db.Stores.AnyAsync(s => s.Id == storeId);
            if (!exists) return Results.NotFound();

            var reservations = await db.Reservations
                .Include(r => r.Inventory).ThenInclude(i => i.Product)
                .Include(r => r.User)
                .Where(r => r.Inventory.StoreId == storeId && r.Status == "pending")
                .Select(r => new ReservationSummaryDto(
                    r.Id, r.ReservationCode,
                    r.Inventory.Product.Name,
                    r.User.Name,
                    r.Status, r.ExpiresAt))
                .ToListAsync();

            return Results.Ok(new StoreReservationsResponse(storeId, reservations));
        });

        app.MapGet("/api/stores/{storeId:guid}/detections", async (Guid storeId, int limit = 10, AppDbContext db = default!) =>
        {
            var exists = await db.Stores.AnyAsync(s => s.Id == storeId);
            if (!exists) return Results.NotFound();

            var raw = await db.Detections
                .Include(d => d.Camera)
                .Where(d => d.Camera.StoreId == storeId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(limit)
                .ToListAsync();

            var detections = raw.Select(d => new DetectionDto(
                d.Id, d.CreatedAt,
                d.DetectedProducts.Select(p => new DetectedProductDto(p.Producto, p.CantidadEstimada)).ToList()))
                .ToList();

            return Results.Ok(new StoreDetectionsResponse(storeId, detections));
        });
    }
}
