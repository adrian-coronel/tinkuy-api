namespace StockLinkApi.Dtos;

public record ProductSearchResponse(string Query, List<ProductSearchResult> Results);

public record ProductSearchResult(
    Guid InventoryId,
    Guid StoreId,
    string StoreName,
    string StoreAddress,
    Guid ProductId,
    string ProductName,
    int StockQuantity,
    decimal Price,
    double DistanceMeters,
    DateTime LastUpdatedAt);
