using EcommerceAPI.Hubs.IHubs;
using EcommerceAPI.Models.Entities;
using Microsoft.AspNetCore.SignalR;

namespace EcommerceAPI.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(ChatMessage message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
}
