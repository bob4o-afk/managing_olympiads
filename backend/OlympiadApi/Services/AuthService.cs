using System.Security.Claims;
using OlympiadApi.DTOs;
using OlympiadApi.Helpers;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(
            IAuthRepository authRepository,
            IJwtHelper jwtHelper,
            IEmailService emailService,
            IConfiguration configuration,
            IUserRepository userRepository)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<object?> LoginAsync(LoginDto loginDto)
        {
            var userDto = await _authRepository.AuthenticateUserAsync(loginDto.UsernameOrEmail, loginDto.Password);
            if (userDto == null) return null;

            var userRolesWithPermissions = await _authRepository.GetUserRolesWithPermissionsAsync(userDto.UserId);
            var user = await _userRepository.FindUserByUsernameOrEmailAsync(userDto.Email);
            if (user == null) return null;

            var token = _jwtHelper.GenerateJwtToken(user, userRolesWithPermissions);

            return new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Username,
                    user.Email
                }
            };
        }

        public async Task<bool> RequestPasswordChangeAsync(PasswordChangeRequestDto requestDto)
        {
            var user = await _authRepository.GetUserByEmailOrUsernameAsync(requestDto.UsernameOrEmail);
            if (user == null) return false;

            var resetToken = Guid.NewGuid().ToString();
            var expiration = DateTime.Now.AddHours(1);
            await _authRepository.StorePasswordResetTokenAsync(user.UserId, resetToken, expiration);

            // Create and send email
            var frontendUrl = _configuration["FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";
            var emailSubject = "Password Reset Request";
            var emailBody = $"To reset your password, click the link below:\n{resetLink}";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, ResetPasswordDto resetPasswordDto)
        {
            var isValid = await _authRepository.ValidateResetTokenAsync(token);
            if (!isValid) return false;

            return await _authRepository.ResetPasswordWithTokenAsync(token, resetPasswordDto.NewPassword);
        }

        public bool ValidateToken(string token)
        {
            return _jwtHelper.ValidateJwtToken(token);
        }

        public async Task<(bool IsValid, string Message)> ValidatePasswordAsync(string token, ValidatePasswordDto validatePasswordDto)
        {
            var claims = _jwtHelper.GetClaimsFromJwt(token);

            if (claims == null || !claims.Any())
            {
                return (false, "Token is invalid.");
            }

            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return (false, "User ID claim not found in token.");
            }

            var isPasswordValid = await _authRepository.ValidateUserPasswordAsync(userId, validatePasswordDto.Password);
            if (!isPasswordValid)
            {
                return (false, "Invalid password.");
            }

            return (true, "Password validated successfully.");
        }
    }
}
