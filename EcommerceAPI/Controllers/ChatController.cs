using EcommerceAPI.Hubs.IHubs;
using EcommerceAPI.Hubs;
using EcommerceAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EcommerceAPI.Models.DTOs.Chat;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using FluentAssertions;

namespace EcommerceAPI.Controllers
{
    [Authorize]
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
        public async Task Post(ChatDTO message)
        {
            var userData = (ClaimsIdentity)User.Identity;
        
            message.Name = userData.FindFirst(ClaimTypes.GivenName).Value;
            message.Surname = userData.FindFirst(ClaimTypes.Surname).Value;
      
            string jsonString = JsonConvert.SerializeObject(message);

            await _chatHub.Clients.All.ReceiveMessage(message);
        }

        

        //[Authorize]
        //[HttpPost("sendToUser")]
        //public async Task SendToUser(ChatDTO message, string receiverConnectionId)
        //{
        //    var userData = (ClaimsIdentity)User.Identity;

        //    message.Name = userData.FindFirst(ClaimTypes.GivenName).Value;
        //    message.Surname = userData.FindFirst(ClaimTypes.Surname).Value;

        //    string jsonString = JsonConvert.SerializeObject(message);

        //    receiverConnectionId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    if (User.IsInRole("LifeUser"))
        //    {
        //        await _chatHub.Clients.User("d71e4112-959d-412a-acb0-cfe8223d280a").SendToUser(message, receiverConnectionId);
        //    }
        //    else if (User.IsInRole("LifeAdmin"))
        //    {
        //        await _chatHub.Clients.User("2452f92b-de5c-40cf-aa3f-8503610386b0").SendToUser(message, receiverConnectionId);
        //    }

        //}

    }
}
