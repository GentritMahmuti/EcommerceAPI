namespace Domain.Entities
{
    public class Inquiry
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
