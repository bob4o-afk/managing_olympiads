using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services;
using OlympiadApi.DTOs;
using OlympiadApi.Helpers;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, JwtHelper jwtHelper, IEmailService emailService, IConfiguration configuration)
        {
            _authService = authService;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _configuration = configuration;
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
            var frontendUrl = _configuration["FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";
            var emailSubject = "Password Reset Request";
            var emailBody = $"To reset your password, click the link below:\n{resetLink}";

            _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return Ok(new { message = "Password reset instructions sent to your email." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, [FromQuery] string token)
        {
            if (resetPasswordDto == null || string.IsNullOrWhiteSpace(resetPasswordDto.Username) || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
            {
                return BadRequest(new { message = "Invalid request. Username and new password are required." });
            }

            try
            {
                var userToken = _authService.GetUserTokenByToken(token);
                if (userToken == null || userToken.Expiration < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Invalid or expired reset token." });
                }

                var user = _authService.GetUserByEmailOrUsername(resetPasswordDto.Username);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var passwordChanged = _authService.ResetPasswordWithToken(token, resetPasswordDto.NewPassword);
                if (!passwordChanged)
                {
                    return StatusCode(500, new { message = "Failed to update password." });
                }

                return Ok(new { message = "Password updated successfully." });
            }
            catch
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost("validate-token")]
        public IActionResult ValidateToken()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Token is missing or invalid." });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                var isValid = _jwtHelper.ValidateJwtToken(token);

                if (!isValid)
                {
                    return Unauthorized(new { message = "Token is invalid or expired." });
                }

                return Ok(new { message = "Token is valid." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating token: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

    }
}
