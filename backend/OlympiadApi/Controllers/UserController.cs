using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Models;
using OlympiadApi.Services;
using OlympiadApi.Helpers;
using OlympiadApi.Filters;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
        }

        [HttpGet]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        //check for matching emails
        [ServiceFilter(typeof(AdminOrStudentRoleAuthorizeAttribute))]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [HttpPost]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
        public IActionResult CreateUser([FromBody] User user)
        {
            _userService.CreateUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        [HttpPut("{id}")]
        //check for matching emails
        [ServiceFilter(typeof(AdminOrStudentRoleAuthorizeAttribute))]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
                return BadRequest(new { message = "User ID mismatch." });

            _userService.UpdateUser(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
        public IActionResult DeleteUser(int id)
        {
            _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
