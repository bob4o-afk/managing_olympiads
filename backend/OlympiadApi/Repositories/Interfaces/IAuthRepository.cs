using OlympiadApi.DTOs;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserDto?> AuthenticateUserAsync(string usernameOrEmail, string password);
        Task<Dictionary<string, Dictionary<string, bool>>> GetUserRolesWithPermissionsAsync(int userId);
        Task<UserDto?> GetUserByEmailOrUsernameAsync(string usernameOrEmail);
        Task StorePasswordResetTokenAsync(int userId, string token, DateTime expiration);
        Task<bool> ResetPasswordWithTokenAsync(string token, string newPassword);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<bool> ValidateUserPasswordAsync(int userId, string password);
    }
}