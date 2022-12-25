using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EcommerceAPI.Models.Entities
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Cover Type")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
