using ExpenseSharingApp.DTOs;
using ExpenseSharingApp.Interfaces;
using ExpenseSharingApp.Models;
using ExpenseSharingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExpenseSharingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;

    public UserController(IUserService userService, IAuthenticationService authenticationService)
    {
        _userService = userService;
        _authenticationService = authenticationService;
    }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var isValid = await _authenticationService.ValidateUserAsync(login.Email, login.Password);
            if (!isValid)
                return Unauthorized();

            var authenticatedUser = await _userService.GetUserByEmailAsync(login.Email);
            var token = await _authenticationService.GenerateJwtTokenAsync(authenticatedUser);

            return Ok(new { Token = token });
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetUserById(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUser(string id, UserDTO userDTO)
        {
            if (!_userService.Update(id, userDTO))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string id)
        {
            if (!_userService.Delete(id))
            {
                return NotFound();
            }
            return NoContent();
        }




    }
}
