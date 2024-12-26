using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public User? GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void CreateUser(User user)
        {
            // Hash the password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null)
                throw new ArgumentException("User not found.");

            // Check if the password is being updated and hash the new password
            if (!string.IsNullOrEmpty(user.Password) && !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Username = user.Username;
            existingUser.Gender = user.Gender;

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
