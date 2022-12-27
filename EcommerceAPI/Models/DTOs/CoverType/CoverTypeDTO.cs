using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.CoverType
{
    public class CoverTypeDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Cover Type")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
