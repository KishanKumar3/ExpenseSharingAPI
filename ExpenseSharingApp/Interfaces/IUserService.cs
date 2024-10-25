using ExpenseSharingApp.DTOs;
using ExpenseSharingApp.Models;

namespace ExpenseSharingApp.Interfaces
{
    public interface IUserService
    {

        Task<User> GetUserByEmailAsync(string email);
        IEnumerable<UserDTO> GetAll();
        UserDTO GetById(string id);

        bool Update(string id, UserDTO user);
        bool Delete(string id);

    }

}
