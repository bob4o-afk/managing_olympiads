using OlympiadApi.DTOs;
using OlympiadApi.Models;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task <IEnumerable<UserDto>> GetAllUsersAsync();
        Task <User?> GetUserByIdAsync(int id);
        Task <UserDto?> GetUserByUsernameAsync(string username);
        Task <User?> FindUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
