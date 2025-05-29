using Moq;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Controllers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.DTOs;
using System.Text.Json;
using OlympiadApi.Helpers;

namespace OlympiadApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockService = new Mock<IUserService>();
            _controller = new UserController(_mockService.Object);
        }

        private User CreateValidUser(int id = 1) => new User
        {
            UserId = id,
            Name = "Test",
            Email = $"test{id}@test.com",
            Password = "123",
            Username = $"testuser{id}",
            DateOfBirth = new DateTime(2000, 1, 1),
            AcademicYearId = 1
        };

        private UserDto CreateValidUserDto(int id = 1) => new UserDto
        {
            UserId = id,
            Name = "Test",
            Email = $"test{id}@test.com",
            Username = $"testuser{id}"
        };

        private Dictionary<string, object> AsDictionary(object? value)
        {
            var json = JsonSerializer.Serialize(value);
            return JsonSerializationHelper.DeserializeFromJson(json)!;
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkWithUsers()
        {
            var users = new List<UserDto> { CreateValidUserDto() };
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, ok.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenFound()
        {
            var user = CreateValidUser();
            _mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

            var result = await _controller.GetUserById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, ok.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.GetUserById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var dict = AsDictionary(notFound.Value);
            Assert.Equal("User not found.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAt()
        {
            var user = CreateValidUser();

            var result = await _controller.CreateUser(user);

            _mockService.Verify(s => s.CreateUserAsync(user), Times.Once);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(user, created.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenValid()
        {
            var user = CreateValidUser();

            var result = await _controller.UpdateUser(1, user);

            _mockService.Verify(s => s.UpdateUserAsync(user), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenIdMismatch()
        {
            var user = CreateValidUser(2);

            var result = await _controller.UpdateUser(1, user);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var dict = AsDictionary(bad.Value);
            Assert.Equal("User ID mismatch.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task UpdateUserNameAndEmail_ReturnsNoContent_WhenValid()
        {
            var user = CreateValidUser();
            var dto = new UserUpdateDto { Name = "New", Email = "new@test.com" };

            _mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

            var result = await _controller.UpdateUserNameAndEmail(1, dto);

            _mockService.Verify(s => s.UpdateUserNameAndEmailAsync(1, dto.Name, dto.Email), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUserNameAndEmail_ReturnsNotFound_WhenMissing()
        {
            var dto = new UserUpdateDto { Name = "New", Email = "new@test.com" };

            _mockService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.UpdateUserNameAndEmail(1, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var dict = AsDictionary(notFound.Value);
            Assert.Equal("User not found.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent()
        {
            var result = await _controller.DeleteUser(1);

            _mockService.Verify(s => s.DeleteUserAsync(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
