using OlympiadApi.DTOs;

namespace OlympiadApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object?> LoginAsync(LoginDto loginDto);
        Task<bool> RequestPasswordChangeAsync(PasswordChangeRequestDto requestDto);
        Task<bool> ResetPasswordAsync(string token, ResetPasswordDto resetPasswordDto);
        bool ValidateToken(string token);
        Task<(bool IsValid, string Message)> ValidatePasswordAsync(string token, ValidatePasswordDto validatePasswordDto);
    }
}
