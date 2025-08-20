using System.ComponentModel.DataAnnotations;

namespace DeepFakeWitness.Models
{
    public class Contact
    {
        [Key]
        public int MessageId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
         public string Message { get; set; }
        public bool IsRead { get; set; } = false;

    }
}
