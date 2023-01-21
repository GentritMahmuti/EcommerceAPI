using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class OrderData
    {
        [Key]
        public string OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public DateTime ShippingDate { get; set; }
        public long OrderTotal { get; set; }
        public string TrackingId { get; set; }
        public string? Carrier { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        [Required]
        public string PhoheNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<ProductOrderData> ProductOrderData { get; set; }
    }
}
