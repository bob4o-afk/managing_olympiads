using Moq;
using OlympiadApi.Controllers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Helpers;
using System.Text.Json;

namespace OlympiadApi.Tests.Controllers
{
    public class RoleControllerTests
    {
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _mockRoleService = new Mock<IRoleService>();
            _controller = new RoleController(_mockRoleService.Object);
        }

        private Dictionary<string, object> AsDictionary(object? value)
        {
            var json = JsonSerializer.Serialize(value);
            return JsonSerializationHelper.DeserializeFromJson(json)!;
        }

        [Fact]
        public async Task GetRoles_ReturnsOk_WithRoles()
        {
            var roles = new List<Role> { new Role { RoleId = 1, RoleName = "Admin" } };
            _mockRoleService.Setup(s => s.GetRolesAsync()).ReturnsAsync(roles);

            var result = await _controller.GetRoles();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<Role>>(okResult.Value!);
            Assert.Single(value);
            Assert.Equal("Admin", value[0].RoleName);
        }

        [Fact]
        public async Task CreateRole_ReturnsOk_WhenValid()
        {
            var role = new Role { RoleId = 2, RoleName = "User" };
            _mockRoleService.Setup(s => s.CreateRoleAsync(role)).ReturnsAsync(role);

            var result = await _controller.CreateRole(role);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(okResult.Value);

            Assert.Equal("Role created successfully", dict["message"]?.ToString());
            Assert.NotNull(dict["role"]);
        }

        [Fact]
        public async Task CreateRole_ReturnsBadRequest_WhenRoleIsInvalid()
        {
            var role = new Role { RoleName = null! };
            _mockRoleService.Setup(s => s.CreateRoleAsync(role)).ReturnsAsync((Role?)null);

            var result = await _controller.CreateRole(role);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var dict = AsDictionary(badRequest.Value);

            Assert.Equal("RoleName is required.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task DeleteRole_ReturnsOk_WhenDeleted()
        {
            _mockRoleService.Setup(s => s.DeleteRoleAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteRole(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(okResult.Value);

            Assert.Equal("Role deleted successfully.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            _mockRoleService.Setup(s => s.DeleteRoleAsync(123)).ReturnsAsync(false);

            var result = await _controller.DeleteRole(123);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var dict = AsDictionary(notFound.Value);

            Assert.Equal("Role not found.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task GetRoles_Returns500_OnException()
        {
            _mockRoleService.Setup(s => s.GetRolesAsync()).ThrowsAsync(new Exception("DB failed"));

            var result = await _controller.GetRoles();

            var error = Assert.IsType<ObjectResult>(result);
            var dict = AsDictionary(error.Value);

            Assert.Equal(500, error.StatusCode);
            Assert.Equal("An error occurred while fetching roles.", dict["message"]?.ToString());
            Assert.Equal("DB failed", dict["error"]?.ToString());
        }

        [Fact]
        public async Task CreateRole_Returns500_OnException()
        {
            var role = new Role { RoleName = "Admin" };
            _mockRoleService.Setup(s => s.CreateRoleAsync(role)).ThrowsAsync(new Exception("Create failed"));

            var result = await _controller.CreateRole(role);

            var error = Assert.IsType<ObjectResult>(result);
            var dict = AsDictionary(error.Value);

            Assert.Equal(500, error.StatusCode);
            Assert.Equal("An error occurred while creating the role.", dict["message"]?.ToString());
            Assert.Equal("Create failed", dict["error"]?.ToString());
        }

        [Fact]
        public async Task DeleteRole_Returns500_OnException()
        {
            _mockRoleService.Setup(s => s.DeleteRoleAsync(1)).ThrowsAsync(new Exception("Delete failed"));

            var result = await _controller.DeleteRole(1);

            var error = Assert.IsType<ObjectResult>(result);
            var dict = AsDictionary(error.Value);

            Assert.Equal(500, error.StatusCode);
            Assert.Equal("An error occurred while deleting the role.", dict["message"]?.ToString());
            Assert.Equal("Delete failed", dict["error"]?.ToString());
        }
    }
}
