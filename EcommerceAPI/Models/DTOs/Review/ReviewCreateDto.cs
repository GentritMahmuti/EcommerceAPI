using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Models.DTOs.Review
{
    public class ReviewCreateDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
    }
}
