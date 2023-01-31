using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Hubs.IHubs
{
    public interface IChatClient
    {
        Task SendMessage(string sender, string receiver, string message);
    }
}
