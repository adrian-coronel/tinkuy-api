using Microsoft.EntityFrameworkCore;
using StockLinkApi.Data;
using StockLinkApi.Dtos;

namespace StockLinkApi.Endpoints;

public static class ProductSearchEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/products/search", async (
            string q,
            double lat,
            double lng,
            double radiusKm = 5,
            AppDbContext db = default!) =>
        {
            var radiusMeters = radiusKm * 1000;

            var items = await db.Inventory
                .Include(i => i.Store)
                .Include(i => i.Product)
                .Where(i => i.StockQuantity > 0
                    && i.Product.Name.ToLower().Contains(q.ToLower()))
                .ToListAsync();

            var results = items
                .Select(i => new
                {
                    Item = i,
                    Distance = Haversine(lat, lng, i.Store.Latitude, i.Store.Longitude)
                })
                .Where(x => x.Distance <= radiusMeters)
                .OrderBy(x => x.Distance)
                .Select(x => new ProductSearchResult(
                    x.Item.Id,
                    x.Item.StoreId,
                    x.Item.Store.Name,
                    x.Item.Store.Address,
                    x.Item.ProductId,
                    x.Item.Product.Name,
                    x.Item.StockQuantity,
                    x.Item.Price,
                    Math.Round(x.Distance, 0),
                    x.Item.UpdatedAt))
                .ToList();

            return Results.Ok(new ProductSearchResponse(q, results));
        });
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
