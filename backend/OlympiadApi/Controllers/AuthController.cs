using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services;
using OlympiadApi.DTOs;
using OlympiadApi.Helpers;
using System.Security.Claims;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly RoleService _roleService;
        private readonly UserRoleAssignmentService _userRoleAssignmentService;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, JwtHelper jwtHelper, IEmailService emailService, IConfiguration configuration, RoleService roleService, UserRoleAssignmentService userRoleAssignmentService, UserService userService)
        {
            _authService = authService;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _configuration = configuration;
            _roleService = roleService;
            _userRoleAssignmentService = userRoleAssignmentService;
            _userService = userService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = _authService.AuthenticateUser(loginDto.UsernameOrEmail, loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var assignments = await _userRoleAssignmentService.GetAllAssignmentsAsync();
            var roles = await _roleService.GetRolesAsync();

            var userRoleIds = assignments
                .Where(a => a.UserId == user.UserId)
                .Select(a => a.RoleId)
                .ToList();

            var userRolesWithPermissions = roles
            .Where(r => userRoleIds.Contains(r.RoleId))
            .ToDictionary(
                r => r.RoleName,
                r => r.Permissions?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value is bool b && b
                ) ?? new Dictionary<string, bool>()
            );

            var token = _jwtHelper.GenerateJwtToken(user, userRolesWithPermissions);

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

        [HttpPost("validate-password")]
        public IActionResult ValidatePassword([FromBody] ValidatePasswordDto validatePasswordDto)
        {
            if (validatePasswordDto == null || string.IsNullOrWhiteSpace(validatePasswordDto.Password))
            {
                return BadRequest(new { message = "Password is required." });
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Token is missing or invalid." });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var claims = _jwtHelper.GetClaimsFromJwt(token);

                if (claims == null || !claims.Any())
                {
                    return Unauthorized(new { message = "Token is invalid." });
                }

                var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User ID claim not found in token." });
                }

                int userId = int.Parse(userIdClaim.Value);

                var isPasswordValid = _jwtHelper.ValidatePassword(userId, validatePasswordDto.Password);  //TO DO: check where should this be
                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Invalid password." });
                }

                return Ok(new { message = "Password validated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating password: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
