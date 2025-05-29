using OlympiadApi.Data;
using OlympiadApi.Models;
using Microsoft.EntityFrameworkCore;
using OlympiadApi.DTOs;

namespace OlympiadApi.Repositories.Interfaces
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> AuthenticateUserAsync(string usernameOrEmail, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<Dictionary<string, Dictionary<string, bool>>> GetUserRolesWithPermissionsAsync(int userId)
        {
            var assignments = await _context.UserRoleAssignments
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var userRoleIds = assignments.Select(a => a.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.RoleId))
                .ToListAsync();

            return roles.ToDictionary(
                r => r.RoleName,
                r => r.Permissions != null
                    ? r.Permissions.ToDictionary(
                        p => p.Key,
                        p => p.Value is bool b && b
                    )
                    : new Dictionary<string, bool>()
            );
        }

        public async Task<UserDto?> GetUserByEmailOrUsernameAsync(string usernameOrEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email
            };
        }
        public async Task StorePasswordResetTokenAsync(int userId, string token, DateTime expiration)
        {
            var existingToken = await _context.UserToken.FirstOrDefaultAsync(ut => ut.UserId == userId);
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

            await _context.UserToken.AddAsync(userToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ResetPasswordWithTokenAsync(string token, string newPassword)
        {
            var userToken = await _context.UserToken.FirstOrDefaultAsync(ut => ut.Token == token);
            if (userToken == null || userToken.Expiration < DateTime.Now)
            {
                if (userToken != null)
                {
                    _context.UserToken.Remove(userToken);
                    await _context.SaveChangesAsync();
                }
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userToken.UserId);
            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            _context.UserToken.Remove(userToken);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            var userToken = await _context.UserToken.FirstOrDefaultAsync(ut => ut.Token == token);
            return userToken != null && userToken.Expiration >= DateTime.Now;
        }

        public async Task<bool> ValidateUserPasswordAsync(int userId, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
    }
}