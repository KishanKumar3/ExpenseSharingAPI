using ExpenseSharingApp.Interfaces;
using ExpenseSharingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Group = ExpenseSharingApp.Models.Group;

namespace ExpenseSharingApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public IActionResult GetAllGroups()
        {
            var groups = _groupService.GetAllGroups();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public IActionResult GetGroupById(string id)
        {
            var group = _groupService.GetGroupById(id);
            if (group == null)
                return NotFound();

            return Ok(group);
        }


        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Group>> GetGroupsByUserId(string userId)
        {
            try
            {
                var groups = _groupService.GetGroupsByUserId(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult AddGroup([FromBody] GroupCreationModel group)
        {
            try
            {
                Group createdGroup = _groupService.CreateGroup(group);
                return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.GroupId }, createdGroup);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteGroup(string id)
        {
            try
            {
                _groupService.DeleteGroup(id);
                return Ok("Group and related expenses deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
