using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Implementations;

namespace OlympiadApi.Tests.Repositories
{
    public class RoleRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"RoleTestDb_{Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RoleRepository(null!));
            Assert.Equal("context", ex.ParamName);
        }

        [Fact]
        public async Task GetRolesAsync_ReturnsAllRoles()
        {
            using var context = GetInMemoryDbContext();
            context.Roles.AddRange(
                new Role { RoleName = "Admin" },
                new Role { RoleName = "User" });
            await context.SaveChangesAsync();

            var repo = new RoleRepository(context);

            var roles = await repo.GetRolesAsync();

            Assert.Equal(2, roles.Count);
            Assert.Contains(roles, r => r.RoleName == "Admin");
            Assert.Contains(roles, r => r.RoleName == "User");
        }

        [Fact]
        public async Task CreateRoleAsync_AddsRole_WhenValid()
        {
            using var context = GetInMemoryDbContext();
            var repo = new RoleRepository(context);
            var role = new Role { RoleName = "Editor" };

            var result = await repo.CreateRoleAsync(role);

            Assert.NotNull(result);
            Assert.Equal("Editor", result.RoleName);
            Assert.Equal(1, context.Roles.Count());
        }

        [Fact]
        public async Task CreateRoleAsync_ReturnsNull_WhenRoleIsNull()
        {
            using var context = GetInMemoryDbContext();
            var repo = new RoleRepository(context);

            var result = await repo.CreateRoleAsync(null!);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateRoleAsync_ReturnsNull_WhenRoleNameIsEmpty()
        {
            using var context = GetInMemoryDbContext();
            var repo = new RoleRepository(context);
            var role = new Role { RoleName = "" };

            var result = await repo.CreateRoleAsync(role);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ReturnsCorrectRole()
        {
            using var context = GetInMemoryDbContext();
            var role = new Role { RoleName = "Manager" };
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var repo = new RoleRepository(context);

            var result = await repo.GetRoleByIdAsync(role.RoleId);

            Assert.NotNull(result);
            Assert.Equal("Manager", result!.RoleName);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ReturnsNull_WhenNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repo = new RoleRepository(context);

            var result = await repo.GetRoleByIdAsync(123);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_RemovesRole_WhenExists()
        {
            using var context = GetInMemoryDbContext();
            var role = new Role { RoleName = "Deleteme" };
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var repo = new RoleRepository(context);
            var result = await repo.DeleteRoleAsync(role.RoleId);

            Assert.True(result);
            Assert.Empty(context.Roles);
        }

        [Fact]
        public async Task DeleteRoleAsync_ReturnsFalse_WhenRoleNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repo = new RoleRepository(context);

            var result = await repo.DeleteRoleAsync(999);

            Assert.False(result);
        }
    }
}
