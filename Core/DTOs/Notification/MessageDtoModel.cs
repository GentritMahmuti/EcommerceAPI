using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Notification
{
    public class MessageDtoModel
    {
        public Guid ConversationGuid { get; set; }

        public string Value { get; set; }
    }
}
