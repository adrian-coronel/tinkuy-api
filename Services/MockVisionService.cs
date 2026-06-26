using StockLinkApi.Models;

namespace StockLinkApi.Services;

public class MockVisionService : IGeminiVisionService
{
    public Task<List<DetectedProductItem>> DetectAsync(string imageBase64, string mimeType = "image/jpeg") =>
        Task.FromResult(new List<DetectedProductItem>
        {
            new() { Producto = "Cargador USB-C 65W",   CantidadEstimada = 8  },
            new() { Producto = "Foco LED 12W",          CantidadEstimada = 15 },
            new() { Producto = "Power Bank 10000mAh",   CantidadEstimada = 4  }
        });
}
