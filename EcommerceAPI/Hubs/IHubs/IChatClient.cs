using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Hubs.IHubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(ChatMessage message);
    }
}
