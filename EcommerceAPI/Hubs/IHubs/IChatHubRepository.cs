using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Hubs.IHubs
{
    public interface IChatHubRepository
    {
        Task SaveMessage(string senderId, string receiverId, string message);
    }
}
