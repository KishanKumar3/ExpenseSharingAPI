

namespace ExpenseSharingApp.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<UserGroupDTO> Groups { get; set; }
        public decimal Balance { get; set; }
    }
}
