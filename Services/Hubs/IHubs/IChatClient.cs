using Services.DTOs.Chat;

namespace Services.Hubs.IHubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(ChatDTO message);

        Task SendToUser(ChatDTO message);
        //Task SendMessage(ChatDTO message);
        //Task ReceiveMessage(ChatDTO message, string receiverConnectionId);
        //Task SendMessage(ChatDTO message);
        //Task<ChatMessage> ReceiveMessage(ChatDTO message);
    }
}
