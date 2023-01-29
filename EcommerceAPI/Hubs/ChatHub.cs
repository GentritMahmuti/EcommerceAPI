using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Hubs.IHubs;
using EcommerceAPI.Models.DTOs.Chat;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace EcommerceAPI.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ChatHub(IMapper mapper, IUnitOfWork unitOfWork) 
        { 
            _mapper= mapper;
            _unitOfWork= unitOfWork;
        }

        //public Task SendPrivateMessage(ChatDTO message)
        //{
        //    return Clients.User().SendAsync("ReceiveMessage", message);
        //}

        public Task SendMessage(ChatDTO message)
        {
            return Clients.All.ReceiveMessage(message);
        }



        //public async Task <ChatMessage> SendMessage(ChatDTO message)
        //{
        //    var chat = _mapper.Map<ChatMessage>(message);

        //    _unitOfWork.Repository<ChatMessage>().Create(chat);
        //    _unitOfWork.Complete();


        //    await Clients.All.ReceiveMessage(message);


        //    return chat;
        //}



        public async Task SendToUser(ChatDTO message, string receiverConnectionId)
        {
            await Clients.Client(receiverConnectionId).ReceiveMessage(message);
        }

        public string GetConnectionID()
        {
            return Context.ConnectionId;
        }
    }
}
