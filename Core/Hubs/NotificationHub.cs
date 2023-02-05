using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Core.Hubs
{
    public class NotificationHub : Hub
    {
        public Task SendNotification(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveNotification", user, message);
        }


        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var currentUserId = "";

            await Groups.AddToGroupAsync(Context.ConnectionId, currentUserId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var currentUserId = "";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentUserId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
