using Nest;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.CoverType
{
    public class CoverTypeDto
    {
        public int Id { get; set; }

        [Display(Name = "Cover Type")]
        public string Name { get; set; }
    }
}
