using Nest;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EcommerceAPI.Models.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required, StringLength(100), Display(Name = "Name")]
        public string CategoryName { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
