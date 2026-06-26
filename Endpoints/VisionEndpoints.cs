using Microsoft.EntityFrameworkCore;
using StockLinkApi.Data;
using StockLinkApi.Models;
using StockLinkApi.Services;

namespace StockLinkApi.Endpoints;

public static class VisionEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/internal/vision/detect", async (VisionDetectRequest req, IGeminiVisionService vision, AppDbContext db) =>
        {
            var camera = await db.Cameras.FindAsync(req.CameraId);
            if (camera is null) return Results.NotFound();

            List<StockLinkApi.Models.DetectedProductItem> detected;
            try
            {
                detected = await vision.DetectAsync(req.ImageBase64, req.MimeType);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("rate limit"))
            {
                return Results.Problem(ex.Message, statusCode: 429);
            }

            var detection = new Detection
            {
                CameraId = req.CameraId,
                DetectedProducts = detected
            };
            db.Detections.Add(detection);

            var productNames = detected.Select(d => d.Producto).ToList();
            var products = await db.Products
                .Where(p => productNames.Contains(p.Name))
                .ToListAsync();

            foreach (var item in detected)
            {
                var product = products.FirstOrDefault(p => p.Name == item.Producto);
                if (product is null) continue;

                var inv = await db.Inventory.FirstOrDefaultAsync(
                    i => i.StoreId == camera.StoreId && i.ProductId == product.Id);
                if (inv is null) continue;

                inv.StockQuantity = item.CantidadEstimada;
                inv.UpdatedAt = DateTime.UtcNow;
            }

            camera.LastSyncAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(new { cameraId = req.CameraId, detectedProducts = detected });
        });
    }
}

record VisionDetectRequest(Guid CameraId, string ImageBase64, string MimeType = "image/jpeg");
