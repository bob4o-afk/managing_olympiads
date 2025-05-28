using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.DTOs;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
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
        public async Task<IActionResult> RequestPasswordChange([FromBody] PasswordChangeRequestDto requestDto)
        {
            var success = await _authService.RequestPasswordChange(requestDto);
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

        [HttpPost("validate-token")]
        public IActionResult ValidateToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token is missing or invalid." });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var isValid = _authService.ValidateToken(token);

            if (!isValid)
            {
                return Unauthorized(new { message = "Token is invalid or expired." });
            }

            return Ok(new { message = "Token is valid." });
        }

        [HttpPost("validate-password")]
        public IActionResult ValidatePassword([FromBody] ValidatePasswordDto validatePasswordDto)
        {
            if (validatePasswordDto == null || string.IsNullOrWhiteSpace(validatePasswordDto.Password))
            {
                return BadRequest(new { message = "Password is required." });
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token is missing or invalid." });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var result = _authService.ValidatePassword(token, validatePasswordDto);

            if (!result.IsValid)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
