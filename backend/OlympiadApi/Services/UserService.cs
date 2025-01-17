using OlympiadApi.DTOs;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User? GetUserById(int id)
        {
            return _userRepository.GetUserById(id);
        }

        // This method returns a UserDto based on the username
        public UserDto? GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        public void CreateUser(User user)
        {
            // Hash password before storing it
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _userRepository.CreateUser(user);
        }

        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteUser(int id)
        {
            _userRepository.DeleteUser(id);
        }
    }
}
