using AutoMapper;
using Core.DTOs.Notification;
using Core.Helpers;
using Core.IServices;
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
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Response<MessageDto>> CreateMessage(string userId, MessageDtoModel request, CancellationToken cancellationToken)
        {
            
            try
            {
                var message = new Message
                {
                    FromUserId = userId,
                    FromUserGuid = Guid.Parse(userId),
                    ConversationGuid = request.ConversationGuid,
                    ConversationId = request.ConversationGuid.ToString(),
                    ToUserId = "d71e4112-959d-412a-acb0-cfe8223d280a",
                    ToUserGuid = Guid.Parse("d71e4112-959d-412a-acb0-cfe8223d280a"),
                    Value = request.Value,
                    CreatedOn = DateTime.Now
                };

                _unitOfWork.Repository<Message>().Create(message);
                _unitOfWork.Complete();

                return new Response<MessageDto>
                {
                    Data = new MessageDto
                    {
                        Id = message.Id,
                        FromUserId = userId,
                        FromUserGuid = Guid.Parse(userId),
                        ConversationGuid = message.ConversationGuid,
                        ConversationId = message.ConversationGuid.ToString(),
                        ToUserId = "d71e4112-959d-412a-acb0-cfe8223d280a",
                        ToUserGuid = Guid.Parse("d71e4112-959d-412a-acb0-cfe8223d280a"),
                        Value = message.Value,
                        CreatedOn = message.CreatedOn
                    },
                    Succeeded = true,
                    Message = "Message created successfully."
                };
            }
            catch (Exception ex)
            {
                return new Response<MessageDto>
                {
                    Succeeded = false,
                    Message = "Error while creating message: " + ex.Message
                };
            }
        }


        public async Task<List<string>> ReadMessages(string userId, Guid conversationGuid, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Id.Equals(userId)).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var messages = _unitOfWork.Repository<Message>().GetByCondition(x => x.ConversationGuid == conversationGuid && x.ToUserId == userId).ToList();

            _unitOfWork.Complete();

            return messages.Select(x => x.Value).ToList();
        }
    }
}
