using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public int OrderDataId { get; set; }
        public OrderData OrderData { get; set; }

        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
