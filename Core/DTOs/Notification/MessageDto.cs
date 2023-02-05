namespace Core.DTOs.Notification
{
    public class MessageDto
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
