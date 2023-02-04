
namespace Services.DTOs.Review
{
    public class ReviewCreateDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
    }
}
