using Nest;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
