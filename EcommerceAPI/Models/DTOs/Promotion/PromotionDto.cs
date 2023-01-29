namespace EcommerceAPI.Models.DTOs.Promotion
{
    public class PromotionDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DiscountAmount { get; set; }
    }
}
