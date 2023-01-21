using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class OrderData
    {
        [Key]
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public long OrderTotal { get; set; }
        public string TrackingId { get; set; }
        public string? Carrier { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public string PhoheNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
