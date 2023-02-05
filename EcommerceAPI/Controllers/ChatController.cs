using Core.Hubs;
using Core.DTOs.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using Core.Helpers;
using Domain.Entities;
using Core.IServices;
using Services.DTOs.Chat;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMessageService _messageService;


        public ChatController(IHubContext<ChatHub> hubContext, IMessageService messageService)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDtoModel request, CancellationToken cancellationToken)
        {
            var response = new Response<MessageDto>();
            var getConnectionId = Request.Headers["connection-id"].ToString() ?? "";

            if (request == null)
            {
                return BadRequest(response.BadRequest());
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            response = await _messageService.CreateMessage(userId, request, cancellationToken);
            if (response == null)
                return BadRequest();

            if (!string.IsNullOrWhiteSpace(response.Message))
            {
                if (response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    return BadRequest(response.Forbidden(response.Message));
                }
                else if (response.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    return BadRequest(response.InternalServerError(response.Message));
                }
                else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    return BadRequest(response.BadRequest(response.Message));
                }
            }

            if (response.Data.FromUserGuid != response.Data.ToUserGuid)
            {
                await _hubContext.Clients.GroupExcept(response.Data.FromUserGuid.ToString(), getConnectionId).SendAsync("ReceiveMessage", response.Data, cancellationToken);
                await _hubContext.Clients.Group(response.Data.ToUserGuid.ToString()).SendAsync("ReceiveMessage", response.Data, cancellationToken);
            }

            return Ok(response.Ok(response.Data));
        }

        [HttpGet("ReadMessages/{conversationGuid}")]
        public async Task<IActionResult> ReadMessages(Guid conversationGuid, CancellationToken cancellationToken)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            await _messageService.ReadMessages(userId, conversationGuid, cancellationToken);

            await _hubContext.Clients.Group("idOfUserWhereToSendSeen").SendAsync("SeenMessage", conversationGuid.ToString(), cancellationToken);

            return Ok();
        }

    }
    
}

