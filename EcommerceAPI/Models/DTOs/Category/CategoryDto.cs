using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.Category
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        [Display(Name = "Name")]
        public string CategoryName { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
