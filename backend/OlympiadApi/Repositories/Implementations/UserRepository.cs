using OlympiadApi.Data;
using OlympiadApi.DTOs;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OlympiadApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users.Select(u => new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Username = u.Username,
                Email = u.Email,
                Gender = u.Gender,
                PersonalSettings = u.PersonalSettings,
                Notifications = u.Notifications,
                CreatedAt = u.CreatedAt
            }).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Gender = user.Gender,
                PersonalSettings = user.PersonalSettings,
                Notifications = user.Notifications,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<User?> FindUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (existingUser == null)
                throw new ArgumentException("User not found.");

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Username = user.Username;
            existingUser.Gender = user.Gender;
            existingUser.PersonalSettings = user.PersonalSettings;
            existingUser.Notifications = user.Notifications;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
