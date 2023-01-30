using EcommerceAPI.Models.DTOs.Order;

namespace EcommerceAPI.Models.DTOs.ShoppingCard
{
    public class ProductSummaryModel
    {
        public AddressDetails AddressDetails { get; set; }

        public string? PromoCode { get; set; }
    }
}
