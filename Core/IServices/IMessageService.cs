using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface IMessageService
    {
        Task SendMessage(string connectionId, string user, string message);
        Task SendMessageToCaller(string connectionId, string user, string message);
        Task SendMessageToGroup(string connectionId, string sender, string receiver, string message);
        Task SendNotification(string connectionId, string user, string message);
    }
}
