using EcommerceAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.OrderDetails
{
    public class OrderDetailsCreateDto
    {
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public OrderData OrderData { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        //public Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
