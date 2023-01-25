namespace EcommerceAPI.Models.DTOs.Promotion
{
    public class PromotionDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DiscountAmount { get; set; }
    }
}
