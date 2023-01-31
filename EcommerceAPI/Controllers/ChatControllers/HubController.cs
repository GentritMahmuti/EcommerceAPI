using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Hubs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceAPI.Controllers.ChatControllers
{
    [Route("/hubs/[controller]")]
    public class HubController : Controller
    {
        private readonly ChatHub _hub;
        private readonly ILogger<ChatHub> _logger;

        public HubController(ChatHub hubContext, ILogger<ChatHub> logger)
        {
            _hub = hubContext;
            _logger = logger;
        }

        [Authorize(Roles = "LifeUser")]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(string recipientId, string message)
        {
            try
            {
                ClaimsPrincipal sender = User;
                if (sender.IsUser())
                {
                    await _hub.Clients.User(recipientId).SendAsync("ReceiveMessage", sender.Identity.Name, message);
                }
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the message to the database");
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("answer")]
        public async Task<IActionResult> SendAnswer(string recipientId, string message)
        {
            ClaimsPrincipal sender = User;
            if (sender.IsAdmin())
            {
                await _hub.Clients.User(recipientId).SendAsync("ReceiveMessage", sender.Identity.Name, message);
            }
            return Ok();
        }

    }
}
