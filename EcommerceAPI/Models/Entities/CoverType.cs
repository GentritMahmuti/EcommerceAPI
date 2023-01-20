using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EcommerceAPI.Models.Entities
{
    public class CoverType
    {
        public int Id { get; set; }

        [Display(Name = "Cover Type")]
        public string Name { get; set; }
    }
}
