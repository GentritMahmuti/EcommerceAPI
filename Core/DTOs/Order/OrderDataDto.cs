﻿using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Order
{
    public class OrderDataDto
    {
        public string OrderId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public DateTime ShippingDate { get; set; }
        public double OrderPrice { get; set; }
        public double OrderFinalPrice { get; set; }
        public string TrackingId { get; set; }
        public string Carrier { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }

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
    }
}
