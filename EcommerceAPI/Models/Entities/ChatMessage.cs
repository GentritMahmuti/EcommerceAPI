using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        
    }
}
