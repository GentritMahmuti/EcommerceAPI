using Nest;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.CoverType
{
    public class CoverTypeCreateDto
    {
        [Display(Name = "Cover Type")]
        public string Name { get; set; }
    }
}
