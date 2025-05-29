using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Implementations;

namespace OlympiadApi.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        private User CreateSampleUser()
        {
            return new User
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "hashedpassword",
                Gender = "Other",
                AcademicYearId = 1,
                PersonalSettings = new Dictionary<string, object> { ["theme"] = "dark" },
                Notifications = new Dictionary<string, object> { ["email"] = true },
                DateOfBirth = new DateTime(2000, 1, 1),
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task CreateUser_AddsUser()
        {
            using var context = GetInMemoryDbContext();
            var repo = new UserRepository(context);

            var user = CreateSampleUser();
            await repo.CreateUserAsync(user);

            Assert.Single(context.Users);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsAllUsers()
        {
            using var context = GetInMemoryDbContext();
            var user1 = CreateSampleUser();
            var user2 = CreateSampleUser();
            user2.Username = "second";
            user2.Email = "second@example.com";
            context.Users.AddRange(user1, user2);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var users = (await repo.GetAllUsersAsync()).ToList();

            Assert.Equal(2, users.Count);
            Assert.All(users, u => Assert.False(string.IsNullOrEmpty(u.Username)));
        }

        [Fact]
        public async Task GetUserById_ReturnsCorrectUser()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var result = await repo.GetUserByIdAsync(user.UserId);

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsDto()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var result = await repo.GetUserByUsernameAsync("testuser");

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsNull_WhenUserNotFound()
        {
            using var context = GetInMemoryDbContext();

            var repo = new UserRepository(context);
            var result = await repo.GetUserByUsernameAsync("nonexistent");

            Assert.Null(result);
        }

        [Fact]
        public async Task FindUserByUsernameOrEmail_ReturnsCorrectUser()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var resultByUsername = await repo.FindUserByUsernameOrEmailAsync("testuser");
            var resultByEmail = await repo.FindUserByUsernameOrEmailAsync("test@example.com");

            Assert.NotNull(resultByUsername);
            Assert.NotNull(resultByEmail);
        }

        [Fact]
        public async Task UpdateUser_ChangesData()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            user.Name = "Updated Name";
            var repo = new UserRepository(context);
            await repo.UpdateUserAsync(user);

            var updated = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            Assert.Equal("Updated Name", updated!.Name);
        }

        [Fact]
        public async Task DeleteUser_RemovesUser()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            await repo.DeleteUserAsync(user.UserId);

            Assert.Empty(context.Users);
        }

        [Fact]
        public async Task UpdateUser_ThrowsException_WhenUserNotFound()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            user.UserId = 999;

            var repo = new UserRepository(context);

            await Assert.ThrowsAsync<ArgumentException>(() => repo.UpdateUserAsync(user));
        }
    }
}
