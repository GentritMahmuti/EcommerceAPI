using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class PaymentMethodEntity
    {
        [Key]
        public string PaymentMethodId { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public string? CustomerId { get; set; }
        public string CardBrand { get; set; }
        public string CardLastFour { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }

    }
}
