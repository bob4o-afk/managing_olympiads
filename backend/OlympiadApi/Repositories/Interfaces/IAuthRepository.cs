using OlympiadApi.DTOs;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserDto?> AuthenticateUserAsync(string usernameOrEmail, string password);
        Task<Dictionary<string, Dictionary<string, bool>>> GetUserRolesWithPermissionsAsync(int userId);
        UserDto? GetUserByEmailOrUsername(string usernameOrEmail);
        void StorePasswordResetToken(int userId, string token, DateTime expiration);
        bool ResetPasswordWithToken(string token, string newPassword);
        bool ValidateUserPassword(int userId, string password);
        bool ValidateResetToken(string token);
    }
}