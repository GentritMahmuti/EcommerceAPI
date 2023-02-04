using Nest;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Core.Hubs
{
    public class EcommerceHub : Hub
    {
        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(string user, string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinChat(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task SendMessageToGroup(string sender, string receiver, string message)
        {
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }

        public Task SendNotificatione(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveNotification", user, message);
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
