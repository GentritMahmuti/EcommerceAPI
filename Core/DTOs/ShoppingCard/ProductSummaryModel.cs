
using Services.DTOs.Order;

namespace Services.DTOs.ShoppingCard
{
    public class ProductSummaryModel
    {
        public AddressDetails AddressDetails { get; set; }

        public string? PromoCode { get; set; }
    }
}
