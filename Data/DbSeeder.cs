using Microsoft.EntityFrameworkCore;
using StockLinkApi.Models;

namespace StockLinkApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Stores.AnyAsync()) return;

        // --- Tiendas (coordenadas reales de Lima) ---
        var stores = new List<Store>
        {
            new() {
                Name = "Tech Store Lince",
                Category = "tecnología",
                Address = "Av. Arequipa 1234, Lince",
                Latitude = -12.0816,
                Longitude = -77.0348,
                Phone = "01-4445566"
            },
            new() {
                Name = "Multiservicios Perú",
                Category = "ferretería",
                Address = "Jr. Lampa 567, Cercado de Lima",
                Latitude = -12.0530,
                Longitude = -77.0282,
                Phone = "01-3334455"
            },
            new() {
                Name = "Electro Miraflores",
                Category = "tecnología",
                Address = "Av. Larco 890, Miraflores",
                Latitude = -12.1191,
                Longitude = -77.0297,
                Phone = "01-2223344"
            }
        };
        db.Stores.AddRange(stores);

        // --- Productos (catálogo curado para el demo) ---
        var products = new List<Product>
        {
            new() { Name = "Cargador USB-C 65W",         Category = "cargadores" },
            new() { Name = "Cargador USB-A 18W",          Category = "cargadores" },
            new() { Name = "Cable USB-C a USB-C 1m",      Category = "cables" },
            new() { Name = "Cable HDMI 2m",               Category = "cables" },
            new() { Name = "Power Bank 10000mAh",         Category = "baterías" },
            new() { Name = "Power Bank 20000mAh",         Category = "baterías" },
            new() { Name = "Foco LED 12W",                Category = "iluminación" },
            new() { Name = "Foco LED 9W",                 Category = "iluminación" },
            new() { Name = "Audífonos Bluetooth",         Category = "audio" },
            new() { Name = "Mouse Inalámbrico",           Category = "periféricos" },
            new() { Name = "Teclado USB",                 Category = "periféricos" },
            new() { Name = "Hub USB 4 puertos",           Category = "conectividad" }
        };
        db.Products.AddRange(products);

        await db.SaveChangesAsync();

        // --- Cámaras (1 por tienda) ---
        var cameras = stores.Select(s => new Camera
        {
            StoreId = s.Id,
            Status = "online",
            LastSyncAt = DateTime.UtcNow
        }).ToList();
        db.Cameras.AddRange(cameras);

        // --- Inventario (stock y precios variados por tienda) ---
        var rng = new Random(42);
        var inventoryItems = new List<Inventory>();

        var stockMatrix = new Dictionary<string, (int minStock, int maxStock, decimal basePrice)>
        {
            ["Cargador USB-C 65W"]       = (3, 12, 45.00m),
            ["Cargador USB-A 18W"]       = (5, 20, 25.00m),
            ["Cable USB-C a USB-C 1m"]   = (8, 30, 15.00m),
            ["Cable HDMI 2m"]            = (4, 15, 20.00m),
            ["Power Bank 10000mAh"]      = (2, 10, 55.00m),
            ["Power Bank 20000mAh"]      = (1,  6, 85.00m),
            ["Foco LED 12W"]             = (6, 25, 18.00m),
            ["Foco LED 9W"]              = (8, 30, 14.00m),
            ["Audífonos Bluetooth"]      = (2,  8, 65.00m),
            ["Mouse Inalámbrico"]        = (3, 10, 35.00m),
            ["Teclado USB"]              = (2,  8, 40.00m),
            ["Hub USB 4 puertos"]        = (3, 12, 28.00m),
        };

        foreach (var store in stores)
        {
            foreach (var product in products)
            {
                if (!stockMatrix.TryGetValue(product.Name, out var data)) continue;
                var priceVariance = (decimal)(rng.NextDouble() * 0.2 - 0.1);
                inventoryItems.Add(new Inventory
                {
                    StoreId = store.Id,
                    ProductId = product.Id,
                    StockQuantity = rng.Next(data.minStock, data.maxStock + 1),
                    Price = Math.Round(data.basePrice * (1 + priceVariance), 2),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        db.Inventory.AddRange(inventoryItems);

        await db.SaveChangesAsync();
    }
}
