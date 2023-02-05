using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Display(Name = "Name")]
        public string CategoryName { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    }
}
