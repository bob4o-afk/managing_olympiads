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
        public void CreateUser_AddsUser()
        {
            using var context = GetInMemoryDbContext();
            var repo = new UserRepository(context);

            var user = CreateSampleUser();
            repo.CreateUser(user);

            Assert.Single(context.Users);
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            using var context = GetInMemoryDbContext();
            var user1 = CreateSampleUser();
            var user2 = CreateSampleUser();
            user2.Username = "second";
            user2.Email = "second@example.com";
            context.Users.AddRange(user1, user2);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var users = repo.GetAllUsers().ToList();

            Assert.Equal(2, users.Count);
            Assert.All(users, u => Assert.False(string.IsNullOrEmpty(u.Username)));
        }

        [Fact]
        public void GetUserById_ReturnsCorrectUser()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var result = repo.GetUserById(user.UserId);

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void GetUserByUsername_ReturnsDto()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var result = repo.GetUserByUsername("testuser");

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public void GetUserByUsername_ReturnsNull_WhenUserNotFound()
        {
            using var context = GetInMemoryDbContext();

            var repo = new UserRepository(context);
            var result = repo.GetUserByUsername("nonexistent");

            Assert.Null(result);
        }

        [Fact]
        public void FindUserByUsernameOrEmail_ReturnsCorrectUser()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            var resultByUsername = repo.FindUserByUsernameOrEmail("testuser");
            var resultByEmail = repo.FindUserByUsernameOrEmail("test@example.com");

            Assert.NotNull(resultByUsername);
            Assert.NotNull(resultByEmail);
        }

        [Fact]
        public void UpdateUser_ChangesData()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            user.Name = "Updated Name";
            var repo = new UserRepository(context);
            repo.UpdateUser(user);

            var updated = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            Assert.Equal("Updated Name", updated!.Name);
        }

        [Fact]
        public void DeleteUser_RemovesUser()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            context.Users.Add(user);
            context.SaveChanges();

            var repo = new UserRepository(context);
            repo.DeleteUser(user.UserId);

            Assert.Empty(context.Users);
        }

        [Fact]
        public void UpdateUser_ThrowsException_WhenUserNotFound()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            user.UserId = 999;

            var repo = new UserRepository(context);

            Assert.Throws<ArgumentException>(() => repo.UpdateUser(user));
        }
    }
}
