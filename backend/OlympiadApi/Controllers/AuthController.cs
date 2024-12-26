using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services;
using OlympiadApi.DTOs;
using OlympiadApi.Helpers;
using OlympiadApi.Models;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;

        public AuthController(AuthService authService, JwtHelper jwtHelper, IEmailService emailService)
        {
            _authService = authService;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _authService.AuthenticateUser(loginDto.UsernameOrEmail, loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = _jwtHelper.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Username,
                    user.Email
                }
            });
        }

        [HttpPost("request-password-change")]
        public IActionResult RequestPasswordChange([FromBody] PasswordChangeRequestDto requestDto)
        {
            var user = _authService.GetUserByEmailOrUsername(requestDto.UsernameOrEmail);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var resetToken = Guid.NewGuid().ToString();
            var expiration = DateTime.UtcNow.AddHours(1);

            _authService.StorePasswordResetToken(user.UserId, resetToken, expiration);

            // Create the reset password link
            var resetLink = $"{Request.Scheme}://{Request.Host}/api/auth/reset-password?token={resetToken}";
            var emailSubject = "Password Reset Request";
            var emailBody = $"To reset your password, click the link below:\n{resetLink}";

            _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return Ok(new { message = "Password reset instructions sent to your email." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ChangePasswordDto changePasswordDto, [FromQuery] string token)
        {
            var userToken = _authService.GetUserTokenByToken(token);
            if (userToken == null || userToken.Expiration < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired reset token." });
            }

            var user = _authService.GetUserByEmailOrUsername(changePasswordDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.Password))
            {
                return Unauthorized(new { message = "Incorrect old password." });
            }

            var passwordChanged = _authService.ChangePasswordWithToken(token, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!passwordChanged)
            {
                return Unauthorized(new { message = "Failed to update password." });
            }

            return Ok(new { message = "Password updated successfully." });
        }


    }
}
