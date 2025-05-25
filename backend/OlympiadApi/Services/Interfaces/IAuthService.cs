using OlympiadApi.DTOs;

namespace OlympiadApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object?> LoginAsync(LoginDto loginDto);
        bool RequestPasswordChange(PasswordChangeRequestDto requestDto);
        bool ResetPassword(string token, ResetPasswordDto resetPasswordDto);
        bool ValidateToken(string token);
        (bool IsValid, string Message) ValidatePassword(string token, ValidatePasswordDto validatePasswordDto);
    }
}
