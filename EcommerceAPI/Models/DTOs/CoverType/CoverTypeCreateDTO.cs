using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.CoverType
{
    public class CoverTypeCreateDTO
    {
        [Display(Name = "Cover Type")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
