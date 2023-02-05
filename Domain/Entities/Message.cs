using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public Guid FromUserGuid { get; set; }
        public int ToUserId { get; set; }
        public Guid ToUserGuid { get; set; }
        public int ConversationId { get; set; }
        public Guid ConversationGuid { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
