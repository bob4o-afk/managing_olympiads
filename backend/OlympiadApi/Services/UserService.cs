using OlympiadApi.DTOs;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        // This method returns a UserDto based on the username
        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task CreateUserAsync(User user)
        {
            // Hash password before storing it
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _userRepository.CreateUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            if(!string.IsNullOrEmpty(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task UpdateUserNameAndEmailAsync(int id, string name, string email)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            user.Name = name;
            user.Email = email;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
