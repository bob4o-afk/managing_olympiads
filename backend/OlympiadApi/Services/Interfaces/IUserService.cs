using OlympiadApi.DTOs;
using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserDto> GetAllUsers();
        User? GetUserById(int id);
        UserDto? GetUserByUsername(string username);
        void CreateUser(User user);
        void UpdateUser(User user);
        void UpdateUserNameAndEmail(int id, string name, string email);
        void DeleteUser(int id);
    }
}
