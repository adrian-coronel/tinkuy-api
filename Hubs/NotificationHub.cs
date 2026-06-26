using Microsoft.AspNetCore.SignalR;

namespace StockLinkApi.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinUser(string userId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, userId);

    public async Task LeaveUser(string userId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
}
