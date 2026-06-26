using StockLinkApi.Models;

namespace StockLinkApi.Services;

public interface IGeminiVisionService
{
    Task<List<DetectedProductItem>> DetectAsync(string imageBase64, string mimeType = "image/jpeg");
}
