using System.Runtime.InteropServices;

namespace ExpenseSharingApp.Models
{
    public class UserGroup
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        //public string Name { get; set; }
        
        //public string Email { get; set; }
        public string GroupId { get; set; }
        public Group Group { get; set; }
        public decimal AmountToPay { get; set; }
    }
}
