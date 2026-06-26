namespace StockLinkApi.Dtos;

public record CreateReservationRequest(Guid UserId, Guid InventoryId);

public record ReservationResponse(
    Guid ReservationId,
    string ReservationCode,
    string Status,
    string StoreName,
    string StoreAddress,
    string ProductName,
    decimal Price,
    DateTime ExpiresAt);
