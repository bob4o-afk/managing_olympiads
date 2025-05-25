using Xunit;
using Moq;
using OlympiadApi.Services;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Models;
using OlympiadApi.DTOs;
using System.Collections.Generic;
using System;

namespace OlympiadApi.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public void GetAllUsers_ReturnsUserDtos()
        {
            var users = new List<UserDto>
            {
                new UserDto { UserId = 1, Username = "test1", Name = "Test One", Email = "test1@example.com" },
                new UserDto { UserId = 2, Username = "test2", Name = "Test Two", Email = "test2@example.com" }
            };
            _userRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(users);

            var result = _userService.GetAllUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetUserById_UserExists_ReturnsUser()
        {
            var user = new User
            {
                UserId = 1,
                Username = "test",
                Name = "Test User",
                Email = "test@example.com",
                Password = "1234",
                DateOfBirth = DateTime.UtcNow.AddYears(-18),
                AcademicYearId = 1
            };
            _userRepositoryMock.Setup(r => r.GetUserById(1)).Returns(user);

            var result = _userService.GetUserById(1);

            Assert.NotNull(result);
            Assert.Equal("test", result.Username);
        }

        [Fact]
        public void GetUserByUsername_ReturnsUserDto()
        {
            var dto = new UserDto
            {
                UserId = 1,
                Username = "john",
                Name = "John Doe",
                Email = "john@example.com"
            };
            _userRepositoryMock.Setup(r => r.GetUserByUsername("john")).Returns(dto);

            var result = _userService.GetUserByUsername("john");

            Assert.NotNull(result);
            Assert.Equal("john", result.Username);
        }

        [Fact]
        public void CreateUser_HashesPassword()
        {
            var user = new User
            {
                Username = "john",
                Name = "John Doe",
                Email = "john@example.com",
                Password = "1234",
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                AcademicYearId = 2
            };

            _userService.CreateUser(user);

            _userRepositoryMock.Verify(r => r.CreateUser(It.Is<User>(u =>
                u.Username == "john" &&
                !string.IsNullOrWhiteSpace(u.Password) &&
                u.Password != "1234"
            )), Times.Once);
        }

        [Fact]
        public void UpdateUser_WithPassword_HashesIt()
        {
            var user = new User
            {
                UserId = 1,
                Username = "editme",
                Name = "Old",
                Email = "old@example.com",
                Password = "secret123",
                DateOfBirth = DateTime.UtcNow.AddYears(-19),
                AcademicYearId = 1
            };

            _userService.UpdateUser(user);

            _userRepositoryMock.Verify(r => r.UpdateUser(It.Is<User>(u =>
                !string.IsNullOrWhiteSpace(u.Password) && u.Password != "secret123"
            )), Times.Once);
        }

        [Fact]
        public void UpdateUser_WithoutPassword_DoesNotHash()
        {
            var user = new User
            {
                UserId = 2,
                Username = "editme",
                Name = "NoPass",
                Email = "nopass@example.com",
                Password = "",
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                AcademicYearId = 2
            };

            _userService.UpdateUser(user);

            _userRepositoryMock.Verify(r => r.UpdateUser(It.Is<User>(u =>
                u.Password == ""
            )), Times.Once);
        }

        [Fact]
        public void UpdateUserNameAndEmail_ValidId_UpdatesUser()
        {
            var user = new User
            {
                UserId = 1,
                Name = "Old",
                Email = "old@example.com",
                Username = "olduser",
                Password = "pass",
                DateOfBirth = DateTime.UtcNow.AddYears(-18),
                AcademicYearId = 1
            };
            _userRepositoryMock.Setup(r => r.GetUserById(1)).Returns(user);

            _userService.UpdateUserNameAndEmail(1, "New", "new@example.com");

            Assert.Equal("New", user.Name);
            Assert.Equal("new@example.com", user.Email);
            _userRepositoryMock.Verify(r => r.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void UpdateUserNameAndEmail_UserNotFound_ThrowsException()
        {
            _userRepositoryMock.Setup(r => r.GetUserById(99)).Returns((User?)null);

            var ex = Assert.Throws<Exception>(() =>
                _userService.UpdateUserNameAndEmail(99, "Test", "test@example.com"));

            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public void DeleteUser_CallsRepository()
        {
            _userService.DeleteUser(5);

            _userRepositoryMock.Verify(r => r.DeleteUser(5), Times.Once);
        }
    }
}
