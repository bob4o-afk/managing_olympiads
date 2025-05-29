using OlympiadApi.DTOs;
using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IUserService
    {
        Task <IEnumerable<UserDto>> GetAllUsersAsync();
        Task <User?> GetUserByIdAsync(int id);
        Task <UserDto?> GetUserByUsernameAsync(string username);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task UpdateUserNameAndEmailAsync(int id, string name, string email);
        Task DeleteUserAsync(int id);
    }
}
