using AutoMapper;
using Persistence.UnitOfWork.IUnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Services.DTOs.Chat;
using Services.Hubs.IHubs;

namespace Services.Hubs
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

        public Task SendMessage(ChatDTO message)
        {
            return Clients.All.ReceiveMessage(message);
        }


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
