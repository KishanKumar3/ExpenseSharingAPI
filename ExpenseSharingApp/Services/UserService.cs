using ExpenseSharingApp.DTOs;
using ExpenseSharingApp.Interfaces;
using ExpenseSharingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseSharingApp.Services
{
    public class UserService : IUserService
    {
        private readonly ExpenseSharingContext _context;

        public UserService(ExpenseSharingContext context)
        {
            _context = context;
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public IEnumerable<UserDTO> GetAll()
        {
            return _context.Users.Select(user => new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Groups = user.Groups.Select(ug => new UserGroupDTO
                {
                    GroupId = ug.GroupId,
                    GroupName = ug.Group.Name
                }).ToList(),
                Balance = user.Balance
            }).ToList();
        }


        public UserDTO GetById(string userId)
        {
            var user = _context.Users
            .Include(u => u.Groups)
            .ThenInclude(ug => ug.Group)
            .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Groups = user.Groups.Select(ug => new UserGroupDTO
                {
                    GroupId = ug.GroupId,
                    GroupName = ug.Group.Name
                }).ToList(),
                Balance = user.Balance
            };
        }

       

        public bool Update(string id, UserDTO userDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;
            user.Role = userDTO.Role;
            user.Balance = userDTO.Balance;

            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        

    }

}
