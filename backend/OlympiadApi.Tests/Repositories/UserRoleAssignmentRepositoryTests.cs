using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Implementations;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services;

namespace OlympiadApi.Tests.Repositories
{
    public class UserRoleAssignmentRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        private User CreateSampleUser() => new User
        {
            Name = "John",
            Username = "john123",
            Email = "john@example.com",
            Password = "password",
            Gender = "Male",
            DateOfBirth = new DateTime(2000, 1, 1),
            AcademicYearId = 1,
            PersonalSettings = new Dictionary<string, object>(),
            Notifications = new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow
        };

        private Role CreateSampleRole() => new Role
        {
            RoleName = "Admin"
        };

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new UserRoleAssignmentRepository(null!));
            Assert.Equal("context", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
        {
            var loggerMock = new Mock<ILogger<UserRoleAssignmentService>>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new UserRoleAssignmentService(null!, loggerMock.Object));

            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            var repoMock = new Mock<IUserRoleAssignmentRepository>();

            var ex = Assert.Throws<ArgumentNullException>(() =>
                new UserRoleAssignmentService(repoMock.Object, null!));

            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public async Task CreateAssignmentAsync_Succeeds_WhenValid()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            var role = CreateSampleRole();
            context.Users.Add(user);
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var repo = new UserRoleAssignmentRepository(context);
            var assignment = new UserRoleAssignment { UserId = user.UserId, RoleId = role.RoleId, AssignedAt = DateTime.UtcNow };

            var result = await repo.CreateAssignmentAsync(assignment);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(role.RoleId, result.RoleId);
        }

        [Fact]
        public async Task CreateAssignmentAsync_Throws_WhenUserOrRoleInvalid()
        {
            using var context = GetInMemoryDbContext();
            var repo = new UserRoleAssignmentRepository(context);
            var assignment = new UserRoleAssignment { UserId = 999, RoleId = 999, AssignedAt = DateTime.UtcNow };

            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.CreateAssignmentAsync(assignment));
        }

        [Fact]
        public async Task GetAllAssignmentsAsync_ReturnsAll()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            var role = CreateSampleRole();
            context.Users.Add(user);
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            context.UserRoleAssignments.Add(new UserRoleAssignment
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var repo = new UserRoleAssignmentRepository(context);
            var result = await repo.GetAllAssignmentsAsync();

            Assert.Single(result);
            Assert.NotNull(result.First().User);
            Assert.NotNull(result.First().Role);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ReturnsCorrectAssignment()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            var role = CreateSampleRole();
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

            var repo = new UserRoleAssignmentRepository(context);
            var result = await repo.GetAssignmentByIdAsync(assignment.AssignmentId);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(role.RoleId, result.RoleId);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_DeletesSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            var user = CreateSampleUser();
            var role = CreateSampleRole();
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

            var repo = new UserRoleAssignmentRepository(context);
            var deleted = await repo.DeleteAssignmentAsync(assignment.AssignmentId);

            Assert.True(deleted);
            Assert.Empty(context.UserRoleAssignments);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ReturnsFalse_WhenNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repo = new UserRoleAssignmentRepository(context);

            var result = await repo.DeleteAssignmentAsync(999);
            Assert.False(result);
        }
    }
}
