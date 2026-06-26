namespace StockLinkApi.Dtos;

public record StoreInventoryResponse(
    Guid StoreId,
    string StoreName,
    string CameraStatus,
    DateTime? LastSyncAt,
    List<InventoryItemDto> Inventory);

public record InventoryItemDto(
    Guid InventoryId,
    string ProductName,
    int StockQuantity,
    decimal Price,
    DateTime UpdatedAt);

public record StoreReservationsResponse(Guid StoreId, List<ReservationSummaryDto> Reservations);

public record ReservationSummaryDto(
    Guid ReservationId,
    string ReservationCode,
    string ProductName,
    string UserName,
    string Status,
    DateTime ExpiresAt);

public record StoreDetectionsResponse(Guid StoreId, List<DetectionDto> Detections);

public record DetectionDto(
    Guid DetectionId,
    DateTime CreatedAt,
    List<DetectedProductDto> DetectedProducts);

public record DetectedProductDto(string Producto, int CantidadEstimada);
