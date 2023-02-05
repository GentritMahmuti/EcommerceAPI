namespace Core.DTOs.Notification
{
    public class NotificationRequestDto
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
