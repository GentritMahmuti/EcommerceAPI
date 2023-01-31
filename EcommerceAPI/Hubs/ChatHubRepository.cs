using EcommerceAPI.Data;
using EcommerceAPI.Hubs.IHubs;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Hubs
{
    public class ChatHubRepository : IChatHubRepository
    {
        private readonly EcommerceDbContext _context;

        public ChatHubRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task SaveMessage(string senderId, string receiverId, string message)
        {
            var textMessage = new TextMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };
            _context.TextMessages.Add(textMessage);
            await _context.SaveChangesAsync();
        }
    }
}
