using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Range(1, 100, ErrorMessage = "")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
