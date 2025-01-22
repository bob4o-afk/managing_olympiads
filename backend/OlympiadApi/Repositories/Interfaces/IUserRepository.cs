using OlympiadApi.DTOs;
using OlympiadApi.Models;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<UserDto> GetAllUsers();
        User? GetUserById(int id);
        UserDto? GetUserByUsername(string username);
        User? FindUserByUsernameOrEmail(string usernameOrEmail);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
    }
}
