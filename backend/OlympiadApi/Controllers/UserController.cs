using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.Filters;
using OlympiadApi.DTOs;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        //check for matching emails
        [RoleAuthorize("Admin", "Student")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.UserId }, user);
        }

        [HttpPut("{id}")]
        //check for matching emails
        [RoleAuthorize("Admin", "Student")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] User user)
        {
            if (id != user.UserId)
                return BadRequest(new { message = "User ID mismatch." });

            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpPatch("{id}")]
        [RoleAuthorize("Admin", "Student")]
        public async Task<IActionResult> UpdateUserNameAndEmailAsync(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            await _userService.UpdateUserNameAndEmailAsync(id, dto.Name, dto.Email);
            return NoContent();
        }


        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
