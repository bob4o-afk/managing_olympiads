using Moq;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services;

namespace OlympiadApi.Tests.Services
{
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _roleService = new RoleService(_roleRepositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RoleService(null!));
            Assert.Equal("roleRepository", ex.ParamName);
        }

        [Fact]
        public async Task GetRolesAsync_ReturnsListOfRoles()
        {
            var roles = new List<Role>
            {
                new Role { RoleName = "Admin" },
                new Role { RoleName = "Student" }
            };
            _roleRepositoryMock.Setup(r => r.GetRolesAsync()).ReturnsAsync(roles);

            var result = await _roleService.GetRolesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateRoleAsync_ValidRole_ReturnsCreatedRole()
        {
            var role = new Role { RoleName = "Teacher" };
            _roleRepositoryMock.Setup(r => r.CreateRoleAsync(role)).ReturnsAsync(role);

            var result = await _roleService.CreateRoleAsync(role);

            Assert.NotNull(result);
            Assert.Equal("Teacher", result.RoleName);
        }

        [Fact]
        public async Task CreateRoleAsync_NullRole_ReturnsNull()
        {
            var result = await _roleService.CreateRoleAsync(null!);

            Assert.Null(result);
            _roleRepositoryMock.Verify(r => r.CreateRoleAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task CreateRoleAsync_EmptyRoleName_ReturnsNull()
        {
            var role = new Role { RoleName = "" };

            var result = await _roleService.CreateRoleAsync(role);

            Assert.Null(result);
            _roleRepositoryMock.Verify(r => r.CreateRoleAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task DeleteRoleAsync_ValidId_ReturnsTrue()
        {
            _roleRepositoryMock.Setup(r => r.DeleteRoleAsync(1)).ReturnsAsync(true);

            var result = await _roleService.DeleteRoleAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_InvalidId_ReturnsFalse()
        {
            _roleRepositoryMock.Setup(r => r.DeleteRoleAsync(999)).ReturnsAsync(false);

            var result = await _roleService.DeleteRoleAsync(999);

            Assert.False(result);
        }
    }
}
