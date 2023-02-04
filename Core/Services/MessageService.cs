using AutoMapper;
using Core.DTOs.Notification;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Persistence.UnitOfWork.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MessageService
    {
        private HubConnection _hubConnection;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //public async Task<Response<MessageDto>> CreateMessage(string userId, MessageDtoModel request, CancellationToken cancellationToken)
        //{
        //    var response = new Response<MessageDto>();

        //    using (var unitOfWork = new UnitOfWork(_dbContext))
        //    {
        //        var message = new TextMessage
        //        {
        //            UserId = userId,
        //            FromUserId = request.FromUserId,
        //            FromUserGuid = request.FromUserGuid,
        //            ToUserId = request.ToUserId,
        //            ToUserGuid = request.ToUserGuid,
        //            ConversationId = request.ConversationId,
        //            ConversationGuid = request.ConversationGuid,
        //            Value = request.Value,
        //            CreatedOn = DateTime.Now
        //        };

        //        unitOfWork.Repository<TextMessage>().Create(message);

        //        var isSaved = await unitOfWork.CompleteAsync();
        //        if (!isSaved)
        //        {
        //            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //            response.Message = "An error occurred while saving the message";
        //            return response;
        //        }

        //        response.Data = new MessageDto
        //        {
        //            Id = message.Id,
        //            FromUserId = message.FromUserId,
        //            FromUserGuid = message.FromUserGuid,
        //            ToUserId = message.ToUserId,
        //            ToUserGuid = message.ToUserGuid,
        //            ConversationId = message.ConversationId,
        //            ConversationGuid = message.ConversationGuid,
        //            Value = message.Value,
        //            CreatedOn = message.CreatedOn
        //        };
        //    }

        //    return response;
        //}

        public async Task SendMessage(string connectionId, string user, string message)
        {
            await _hubConnection.InvokeAsync("SendMessage", user, message);
        }

        public async Task SendMessageToCaller(string connectionId, string user, string message)
        {
            await _hubConnection.InvokeAsync("SendMessageToCaller", user, message);
        }

        public async Task SendMessageToGroup(string connectionId, string sender, string receiver, string message)
        {
            await _hubConnection.InvokeAsync("SendMessageToGroup", sender, receiver, message);
        }

        public async Task SendNotification(string connectionId, string user, string message)
        {
            await _hubConnection.InvokeAsync("SendNotification", user, message);
        }
    }
}
