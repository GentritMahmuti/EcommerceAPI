using Core.DTOs.Notification;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface IMessageService
    {
        Task<Response<MessageDto>> CreateMessage(string userId, MessageDtoModel request, CancellationToken cancellationToken);
        Task ReadMessages(string userId, Guid conversationGuid, CancellationToken cancellationToken);
    }
}
