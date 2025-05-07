using Microsoft.AspNetCore.SignalR;

namespace EvrenDev.PublicApi.Hub;
public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
