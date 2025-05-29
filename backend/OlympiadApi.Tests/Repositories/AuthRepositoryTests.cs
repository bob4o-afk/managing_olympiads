using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Tests.Repositories
{
    public class AuthRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        private User CreateUser(string username = "user", string email = "user@example.com", string password = "Password123")
        {
            return new User
            {
                Username = username,
                Email = email,
                Name = "Test User",
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Gender = "Other",
                AcademicYearId = 1,
                DateOfBirth = new DateTime(2000, 1, 1),
                CreatedAt = DateTime.Now,
                Notifications = new(),
                PersonalSettings = new()
            };
        }

        [Fact]
        public async Task AuthenticateUserAsync_ReturnsUser_WhenValid()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new AuthRepository(context);
            var result = await repo.AuthenticateUserAsync("user", "Password123");

            Assert.NotNull(result);
            Assert.Equal("user", result.Username);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ReturnsNull_WhenInvalidUsernameOrPassword()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new AuthRepository(context);

            var badUser = await repo.AuthenticateUserAsync("wrong", "Password123");
            var badPass = await repo.AuthenticateUserAsync("user", "WrongPassword");

            Assert.Null(badUser);
            Assert.Null(badPass);
        }

        [Fact]
        public async Task GetUserRolesWithPermissionsAsync_ReturnsCorrectPermissions()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            var role = new Role { RoleName = "Admin", Permissions = new Dictionary<string, object> { ["create_users"] = true, ["enroll_olympiads"] = false } };

            context.Users.Add(user);
            context.Roles.Add(role);
            context.SaveChanges();

            var assignment = new UserRoleAssignment
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.Now
            };
            context.UserRoleAssignments.Add(assignment);
            context.SaveChanges();

            var repo = new AuthRepository(context);
            var result = await repo.GetUserRolesWithPermissionsAsync(user.UserId);

            Assert.True(result.ContainsKey("Admin"));
            Assert.True(result["Admin"]["create_users"]);
            Assert.False(result["Admin"]["enroll_olympiads"]);
        }

        [Fact]
        public async Task GetUserByEmailOrUsername_ReturnsUserDto_WhenExists()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new AuthRepository(context);

            var byUsername = await repo.GetUserByEmailOrUsernameAsync("user");
            var byEmail = await repo.GetUserByEmailOrUsernameAsync("user@example.com");

            Assert.NotNull(byUsername);
            Assert.NotNull(byEmail);
        }

        [Fact]
        public async Task GetUserByEmailOrUsername_ReturnsNull_WhenNotExists()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AuthRepository(context);
            var result = await repo.GetUserByEmailOrUsernameAsync("notfound");
            Assert.Null(result);
        }

        [Fact]
        public async Task StorePasswordResetToken_CreatesToken()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new AuthRepository(context);
            await repo.StorePasswordResetTokenAsync(user.UserId, "token123", DateTime.Now.AddHours(1));

            Assert.Single(context.UserToken);
            Assert.Equal("token123", context.UserToken.First().Token);
        }


        [Fact]
        public async Task ValidateResetToken_ReturnsTrue_WhenValid()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var userId = user.UserId;

            context.UserToken.Add(new UserToken
            {
                UserId = userId,
                Token = "token123",
                Expiration = DateTime.Now.AddMinutes(5)
            });
            context.SaveChanges();

            var repo = new AuthRepository(context);
            Assert.True(await repo.ValidateResetTokenAsync("token123"));
        }

        [Fact]
        public async Task ValidateResetToken_ReturnsFalse_WhenInvalidOrExpired()
        {
            using var context = GetInMemoryDbContext();
            context.UserToken.Add(new UserToken
            {
                UserTokenId = 1,
                UserId = 1,
                Token = "expired",
                Expiration = DateTime.Now.AddSeconds(-1),
                User = CreateUser()
            });
            context.SaveChanges();

            var repo = new AuthRepository(context);
            Assert.False(await repo.ValidateResetTokenAsync("expired"));
            Assert.False(await repo.ValidateResetTokenAsync("nonexistent"));
        }

        [Fact]
        public async Task ValidateUserPassword_ReturnsTrue_WhenPasswordMatches()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new AuthRepository(context);
            Assert.True(await repo.ValidateUserPasswordAsync(user.UserId, "Password123"));
        }

        [Fact]
        public async Task ValidateUserPassword_ReturnsFalse_WhenNoUserOrWrongPassword()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new AuthRepository(context);
            Assert.False(await repo.ValidateUserPasswordAsync(user.UserId, "WrongPass"));
            Assert.False(await repo.ValidateUserPasswordAsync(9999, "Password123"));
        }

        [Fact]
        public async Task GetUserRolesWithPermissionsAsync_ReturnsEmpty_WhenNoRolesAssigned()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new AuthRepository(context);
            var result = await repo.GetUserRolesWithPermissionsAsync(user.UserId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserRolesWithPermissionsAsync_ReturnsEmptyPermissions_WhenPermissionsNull()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            var role = new Role { RoleName = "EmptyPermissions", Permissions = null! };
            context.Users.Add(user);
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var assignment = new UserRoleAssignment
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.UtcNow
            };
            context.UserRoleAssignments.Add(assignment);
            await context.SaveChangesAsync();

            var repo = new AuthRepository(context);
            var result = await repo.GetUserRolesWithPermissionsAsync(user.UserId);

            Assert.True(result.ContainsKey("EmptyPermissions"));
            Assert.Empty(result["EmptyPermissions"]);
        }

        [Fact]
        public async Task StorePasswordResetToken_ReplacesExistingToken()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.UserToken.Add(new UserToken
            {
                UserId = user.UserId,
                Token = "oldToken",
                Expiration = DateTime.Now.AddHours(1)
            });
            context.SaveChanges();

            var repo = new AuthRepository(context);
            await repo.StorePasswordResetTokenAsync(user.UserId, "newToken", DateTime.Now.AddHours(2));

            var token = context.UserToken.FirstOrDefault(ut => ut.UserId == user.UserId);
            Assert.NotNull(token);
            Assert.Equal("newToken", token!.Token);
        }

        [Fact]
        public async Task ResetPasswordWithToken_ReturnsFalse_WhenTokenIsExpired()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.UserToken.Add(new UserToken
            {
                UserId = user.UserId,
                Token = "expiredToken",
                Expiration = DateTime.Now.AddMinutes(-5)
            });
            context.SaveChanges();

            var repo = new AuthRepository(context);
            var result = await repo.ResetPasswordWithTokenAsync("expiredToken", "newPass");

            Assert.False(result);
            Assert.Empty(context.UserToken);
        }

        [Fact]
        public async Task ResetPasswordWithToken_ReturnsFalse_WhenTokenNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AuthRepository(context);

            var result = await repo.ResetPasswordWithTokenAsync("nonexistent", "newPass");
            Assert.False(result);
        }

        [Fact]
        public async Task ResetPasswordWithToken_ReturnsFalse_WhenUserNotFound()
        {
            using var context = GetInMemoryDbContext();
            context.UserToken.Add(new UserToken
            {
                UserId = 9999,
                Token = "validToken",
                Expiration = DateTime.Now.AddMinutes(10)
            });
            context.SaveChanges();

            var repo = new AuthRepository(context);
            var result = await repo.ResetPasswordWithTokenAsync("validToken", "newPass");

            Assert.False(result);
        }

        [Fact]
        public async Task ResetPasswordWithToken_ResetsPassword_WhenValid()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateUser();
            context.Users.Add(user);
            context.UserToken.Add(new UserToken
            {
                UserId = user.UserId,
                Token = "validToken",
                Expiration = DateTime.Now.AddMinutes(10)
            });
            context.SaveChanges();

            var repo = new AuthRepository(context);
            var result = await repo.ResetPasswordWithTokenAsync("validToken", "newSecurePass");

            Assert.True(result);
            var updatedUser = context.Users.First(u => u.UserId == user.UserId);
            Assert.True(BCrypt.Net.BCrypt.Verify("newSecurePass", updatedUser.Password));
            Assert.Empty(context.UserToken);
        }

    }
}
