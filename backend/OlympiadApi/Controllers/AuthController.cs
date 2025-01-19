using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services;
using OlympiadApi.DTOs;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(response);
        }

        [HttpPost("request-password-change")]
        public IActionResult RequestPasswordChange([FromBody] PasswordChangeRequestDto requestDto)
        {
            var success = _authService.RequestPasswordChange(requestDto);
            if (!success)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new { message = "Password reset instructions sent to your email." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, [FromQuery] string token)
        {
            var success = _authService.ResetPassword(token, resetPasswordDto);
            if (!success)
            {
                return BadRequest(new { message = "Invalid or expired reset token." });
            }

            return Ok(new { message = "Password updated successfully." });
        }
    }
}
