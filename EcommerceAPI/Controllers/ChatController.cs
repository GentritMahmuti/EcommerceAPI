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

namespace EcommerceAPI.Controllers
{
    [ApiController]
    public class ChatController : Controller
    {
        //private readonly IHubContext<ChatHub> _hubContext;
        //private readonly IMessageService _messageService;


        //public ChatController(
        //                       IHubContext<ChatHub> hubContext,
        //                       IMessageService messageService
        //                      )
        //{
        //    _messageService = messageService;
        //    _hubContext = hubContext;
        //}


        /// <summary>
        /// Posts a new chat message to the chat system.
        /// </summary>
        /// <param name="message">The chat message to be posted</param>
        //[HttpPost("messages")]
        //public async Task Post(ChatDTO message)
        //{
        //    var userData = (ClaimsIdentity)User.Identity;
        
        //    message.Name = userData.FindFirst(ClaimTypes.GivenName).Value;
        //    message.Surname = userData.FindFirst(ClaimTypes.Surname).Value;
      
        //    string jsonString = JsonConvert.SerializeObject(message);

        //    if (!string.IsNullOrWhiteSpace(response.Message))
        //    {
        //        if (response.StatusCode == (int)HttpStatusCode.Forbidden)
        //        {
        //            return BadRequest(response.Forbidden(response.Message));
        //        }
        //        else if (response.StatusCode == (int)HttpStatusCode.InternalServerError)
        //        {
        //            return BadRequest(response.InternalServerError(response.Message));
        //        }
        //        else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
        //        {
        //            return BadRequest(response.BadRequest(response.Message));
        //        }
        //    }

        //    if (response.Data.FromUserGuid != response.Data.ToUserGuid)
        //    {
        //        await _hubContext.Clients.GroupExcept(response.Data.FromUserGuid.ToString(), getConnectionId).SendAsync("ReceiveMessage", response.Data, cancellationToken);
        //        await _hubContext.Clients.Group(response.Data.ToUserGuid.ToString()).SendAsync("ReceiveMessage", response.Data, cancellationToken);
        //    }

        //    return Ok(response.Ok(response.Data));
        //}

        //[HttpGet("ReadMessages/{conversationGuid}")]
        //public async Task<IActionResult> ReadMessages(Guid conversationGuid, CancellationToken cancellationToken)
        //{
        //    var response = await _messageService.ReadMessages((userId, conversationGuid, cancellationToken);

        //    await _hubContext.Clients.Group("idOfUserWhereToSendSeen").SendAsync("SeenMessage", conversationGuid.ToString(), cancellationToken);

        //    return Ok();
        //}

    }
    
}

