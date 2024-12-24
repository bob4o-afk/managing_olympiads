using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Helpers; 


namespace OlympiadApi.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public UserService(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context; 
            _jwtHelper = jwtHelper;
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

            // If the password is updated, hash it
            if (!string.IsNullOrEmpty(user.Password) && user.Password != existingUser.Password)
            {
                user.Password = PasswordHelper.HashPassword(user.Password);
            }

            _context.Users.Update(user);
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

        public string? AuthenticateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return null;

            // Verify the password
            if (!PasswordHelper.VerifyPassword(password, user.Password))
                return null;

            return _jwtHelper.GenerateJwtToken(user);
        }
    }
}
