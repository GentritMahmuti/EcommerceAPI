using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class NotificationRequest
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int NotificationType { get; set; }
        public int ReceiverId { get; set; }
        public Guid ReceiverGuid { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public string RedirectUrl { get; set; }
        public DateTime EventDateTime { get; set; }
        public bool Seen { get; set; }
    }
}
