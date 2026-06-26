using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using StockLinkApi.Models;

namespace StockLinkApi.Services;

public class GeminiVisionService(HttpClient http, IConfiguration config) : IGeminiVisionService
{
    private static readonly string[] KnownProducts =
    [
        "Cargador USB-C 65W", "Cargador USB-A 18W", "Cable USB-C a USB-C 1m",
        "Cable HDMI 2m", "Power Bank 10000mAh", "Power Bank 20000mAh",
        "Foco LED 12W", "Foco LED 9W", "Audífonos Bluetooth",
        "Mouse Inalámbrico", "Teclado USB", "Hub USB 4 puertos"
    ];

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<DetectedProductItem>> DetectAsync(string imageBase64, string mimeType = "image/jpeg")
    {
        var apiKey = config["GeminiApiKey"]
            ?? throw new InvalidOperationException("GeminiApiKey no configurada");

        var productList = string.Join(", ", KnownProducts);
        var prompt = $$"""
            Analiza esta imagen de un anaquel de tienda. Identifica cada producto visible de esta lista: {{productList}}.
            Estima la cantidad de unidades visibles por producto.
            Devuelve SOLO un JSON con este formato exacto, sin texto adicional ni bloques de código markdown:
            [{"producto": "nombre exacto del catálogo", "cantidad_estimada": numero}]
            """;

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = prompt },
                        new { inlineData = new { mimeType, data = imageBase64 } }
                    }
                }
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

        var response = await CallWithRetryAsync(url, body);

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(JsonOpts);
        var raw = result?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "[]";
        var json = Regex.Replace(raw, @"```(?:json)?\s*|\s*```", "").Trim();

        return JsonSerializer.Deserialize<List<DetectedProductItem>>(json, JsonOpts) ?? [];
    }

    private async Task<HttpResponseMessage> CallWithRetryAsync(string url, object body)
    {
        var response = await http.PostAsJsonAsync(url, body);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            // Respeta Retry-After si Gemini lo manda, si no espera 5 segundos
            var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5);
            await Task.Delay(retryAfter);
            response = await http.PostAsJsonAsync(url, body);
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new InvalidOperationException("Gemini rate limit alcanzado. Intenta de nuevo en unos segundos.");

        response.EnsureSuccessStatusCode();
        return response;
    }
}

file record GeminiResponse(GeminiCandidate[]? Candidates);
file record GeminiCandidate(GeminiContent? Content);
file record GeminiContent(GeminiPart[]? Parts);
file record GeminiPart(string? Text);
