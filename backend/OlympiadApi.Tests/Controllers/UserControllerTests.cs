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
        public void GetAllUsers_ReturnsOkWithUsers()
        {
            var users = new List<UserDto> { CreateValidUserDto() };
            _mockService.Setup(s => s.GetAllUsers()).Returns(users);

            var result = _controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, ok.Value);
        }

        [Fact]
        public void GetUserById_ReturnsOk_WhenFound()
        {
            var user = CreateValidUser();
            _mockService.Setup(s => s.GetUserById(1)).Returns(user);

            var result = _controller.GetUserById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, ok.Value);
        }

        [Fact]
        public void GetUserById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetUserById(1)).Returns((User?)null);

            var result = _controller.GetUserById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var dict = AsDictionary(notFound.Value);
            Assert.Equal("User not found.", dict["message"]?.ToString());
        }

        [Fact]
        public void CreateUser_ReturnsCreatedAt()
        {
            var user = CreateValidUser();

            var result = _controller.CreateUser(user);

            _mockService.Verify(s => s.CreateUser(user), Times.Once);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(user, created.Value);
        }

        [Fact]
        public void UpdateUser_ReturnsNoContent_WhenValid()
        {
            var user = CreateValidUser();

            var result = _controller.UpdateUser(1, user);

            _mockService.Verify(s => s.UpdateUser(user), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateUser_ReturnsBadRequest_WhenIdMismatch()
        {
            var user = CreateValidUser(2);

            var result = _controller.UpdateUser(1, user);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var dict = AsDictionary(bad.Value);
            Assert.Equal("User ID mismatch.", dict["message"]?.ToString());
        }

        [Fact]
        public void UpdateUserNameAndEmail_ReturnsNoContent_WhenValid()
        {
            var user = CreateValidUser();
            var dto = new UserUpdateDto { Name = "New", Email = "new@test.com" };

            _mockService.Setup(s => s.GetUserById(1)).Returns(user);

            var result = _controller.UpdateUserNameAndEmail(1, dto);

            _mockService.Verify(s => s.UpdateUserNameAndEmail(1, dto.Name, dto.Email), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateUserNameAndEmail_ReturnsNotFound_WhenMissing()
        {
            var dto = new UserUpdateDto { Name = "New", Email = "new@test.com" };

            _mockService.Setup(s => s.GetUserById(1)).Returns((User?)null);

            var result = _controller.UpdateUserNameAndEmail(1, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var dict = AsDictionary(notFound.Value);
            Assert.Equal("User not found.", dict["message"]?.ToString());
        }

        [Fact]
        public void DeleteUser_ReturnsNoContent()
        {
            var result = _controller.DeleteUser(1);

            _mockService.Verify(s => s.DeleteUser(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
