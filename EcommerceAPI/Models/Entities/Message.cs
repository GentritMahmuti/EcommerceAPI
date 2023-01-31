namespace EcommerceAPI.Models.Entities
{
    public class TextMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; } = DateTime.Now;
    }
}
