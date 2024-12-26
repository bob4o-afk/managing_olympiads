using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Authenticate user by email or username
        public User? AuthenticateUser(string usernameOrEmail, string password)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }

        public void StorePasswordResetToken(int userId, string token, DateTime expiration)
        {
            // Check if a token already exists for the user and remove it before adding the new one
            var existingToken = _context.UserToken.FirstOrDefault(ut => ut.UserId == userId);
            if (existingToken != null)
            {
                _context.UserToken.Remove(existingToken);
            }

            var userToken = new UserToken
            {
                UserId = userId,
                Token = token,
                Expiration = expiration
            };

            _context.UserToken.Add(userToken);
            _context.SaveChanges();
        }

        public UserToken? GetUserTokenByToken(string token)
        {
            return _context.UserToken.FirstOrDefault(ut => ut.Token == token);
        }

        public void RemoveUserToken(int userId)
        {
            var userToken = _context.UserToken.FirstOrDefault(ut => ut.UserId == userId);
            if (userToken != null)
            {
                _context.UserToken.Remove(userToken);
                _context.SaveChanges();
            }
        }

        public User? GetUserByEmailOrUsername(string usernameOrEmail)
        {
            return _context.Users.FirstOrDefault(u =>
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public bool ChangePasswordWithToken(string token, string oldPassword, string newPassword)
        {
            var userToken = _context.UserToken.FirstOrDefault(ut => ut.Token == token);

            if (userToken == null || userToken.Expiration < DateTime.UtcNow)
            {
                return false;
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userToken.UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            _context.SaveChanges();

            // Remove the token after the password change
            RemoveUserToken(user.UserId);

            return true;
        }
    }
}
