namespace Core.DTOs.Notification
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string? FromUserId { get; set; }
        public Guid? FromUserGuid { get; set; }
        public string? ToUserId { get; set; }
        public Guid? ToUserGuid { get; set; }
        public string ConversationId { get; set; }
        public Guid ConversationGuid { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
