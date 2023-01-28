using EcommerceAPI.Hubs.IHubs;
using EcommerceAPI.Hubs;
using EcommerceAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;

        public ChatController(IHubContext<ChatHub, IChatClient> chatHub)
        {
            _chatHub = chatHub;
        }

        
        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            // run some logic...

            await _chatHub.Clients.All.ReceiveMessage(message);
        }
    }
}
