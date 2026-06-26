namespace StockLinkApi.Dtos;

public record NotificationResponse(
    Guid Id,
    Guid? UserId,
    string Type,
    string Title,
    string Message,
    string? Payload,
    bool IsRead,
    DateTime CreatedAt);

public record TestNotificationRequest(Guid? UserId, string Type, string Title, string Message);
