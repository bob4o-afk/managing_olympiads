using OlympiadApi.DTOs;
using OlympiadApi.Helpers;
using OlympiadApi.Repositories;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper, IEmailService emailService, IConfiguration configuration, IUserRepository userRepository)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<object?> LoginAsync(LoginDto loginDto)
        {
            var userdto = await _authRepository.AuthenticateUserAsync(loginDto.UsernameOrEmail, loginDto.Password);
            if (userdto == null) return null;

            var userRolesWithPermissions = await _authRepository.GetUserRolesWithPermissionsAsync(userdto.UserId);
            var user = _userRepository.FindUserByUsernameOrEmail(userdto.Email);
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

        public bool RequestPasswordChange(PasswordChangeRequestDto requestDto)
        {
            var user = _authRepository.GetUserByEmailOrUsername(requestDto.UsernameOrEmail);
            if (user == null) return false;

            var resetToken = Guid.NewGuid().ToString();
            var expiration = DateTime.Now.AddHours(1);
            _authRepository.StorePasswordResetToken(user.UserId, resetToken, expiration);

            // Create and send email
            var frontendUrl = _configuration["FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";
            var emailSubject = "Password Reset Request";
            var emailBody = $"To reset your password, click the link below:\n{resetLink}";
            _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return true;
        }

        public bool ResetPassword(string token, ResetPasswordDto resetPasswordDto)
        {
            if (!_authRepository.ValidateResetToken(token))
            {
                return false;
            }

            return _authRepository.ResetPasswordWithToken(token, resetPasswordDto.NewPassword);
        }

        public bool ValidateToken(string token)
        {
            return _jwtHelper.ValidateJwtToken(token);
        }

        public (bool IsValid, string Message) ValidatePassword(string token, ValidatePasswordDto validatePasswordDto)
        {
            var claims = _jwtHelper.GetClaimsFromJwt(token);

            if (claims == null || !claims.Any())
            {
                return (false, "Token is invalid.");
            }

            var userIdClaim = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return (false, "User ID claim not found in token.");
            }

            var isPasswordValid = _authRepository.ValidateUserPassword(userId, validatePasswordDto.Password);
            if (!isPasswordValid)
            {
                return (false, "Invalid password.");
            }

            return (true, "Password validated successfully.");
        }

    }
}
