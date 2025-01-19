using OlympiadApi.DTOs;
using OlympiadApi.Helpers;
using OlympiadApi.Repositories;

namespace OlympiadApi.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper, IEmailService emailService, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<object?> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.AuthenticateUserAsync(loginDto.UsernameOrEmail, loginDto.Password);
            if (user == null) return null;

            var userRolesWithPermissions = await _authRepository.GetUserRolesWithPermissionsAsync(user.UserId);
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
    }
}
