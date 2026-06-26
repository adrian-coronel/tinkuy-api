using Microsoft.EntityFrameworkCore;
using StockLinkApi.Data;
using StockLinkApi.Dtos;
using StockLinkApi.Models;

namespace StockLinkApi.Endpoints;

public static class ReservationEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/reservations", async (CreateReservationRequest req, AppDbContext db) =>
        {
            var inventory = await db.Inventory
                .Include(i => i.Store)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == req.InventoryId);

            if (inventory is null)
                return Results.NotFound();

            if (inventory.StockQuantity <= 0)
                return Results.Conflict(new { error = "OUT_OF_STOCK", message = "El producto ya no está disponible en esta tienda" });

            var reservation = new Reservation
            {
                UserId = req.UserId,
                InventoryId = req.InventoryId,
                ReservationCode = $"SL-{Random.Shared.Next(1000, 9999)}",
                Status = "pending",
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();

            return Results.Created($"/api/reservations/{reservation.Id}",
                ToResponse(reservation, inventory));
        });

        app.MapGet("/api/reservations/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var reservation = await db.Reservations
                .Include(r => r.Inventory).ThenInclude(i => i.Store)
                .Include(r => r.Inventory).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            return reservation is null
                ? Results.NotFound()
                : Results.Ok(ToResponse(reservation, reservation.Inventory));
        });
    }

    private static ReservationResponse ToResponse(Models.Reservation r, Models.Inventory i) =>
        new(r.Id, r.ReservationCode, r.Status,
            i.Store.Name, i.Store.Address,
            i.Product.Name, i.Price, r.ExpiresAt);
}
