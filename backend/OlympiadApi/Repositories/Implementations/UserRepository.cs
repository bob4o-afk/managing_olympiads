using OlympiadApi.Data;
using OlympiadApi.DTOs;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            return _context.Users.Select(u => new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Username = u.Username,
                Email = u.Email,
                Gender = u.Gender,
                PersonalSettings = u.PersonalSettings,
                Notifications = u.Notifications,
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public UserDto? GetUserByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
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

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null)
                throw new ArgumentException("User not found.");

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Username = user.Username;
            existingUser.Gender = user.Gender;
            existingUser.PersonalSettings = user.PersonalSettings;
            existingUser.Notifications = user.Notifications;

            _context.Users.Update(existingUser);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
