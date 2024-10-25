using System.ComponentModel.DataAnnotations;

namespace ExpenseSharingApp.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin or Normal


        public ICollection<UserGroup> Groups { get; set; } = new List<UserGroup>();

        public decimal Balance { get; set; }

        
    }
}
