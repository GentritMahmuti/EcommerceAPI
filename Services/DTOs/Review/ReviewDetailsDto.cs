namespace Services.DTOs.Review
{
    public class ReviewDetailsDto
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
        public DateTime ReviewPostedDate { get; set; }
    }
}
