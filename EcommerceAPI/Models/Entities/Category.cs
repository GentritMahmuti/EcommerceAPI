using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EcommerceAPI.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
