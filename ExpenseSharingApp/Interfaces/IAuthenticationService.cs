using ExpenseSharingApp.Models;

namespace ExpenseSharingApp.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<bool> ValidateUserAsync(string email, string password);
    }
}
