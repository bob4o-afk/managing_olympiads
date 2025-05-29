using Moq;
using OlympiadApi.Services;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Models;
using OlympiadApi.DTOs;

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
        public async Task GetAllUsers_ReturnsUserDtos()
        {
            var users = new List<UserDto>
            {
                new UserDto { UserId = 1, Username = "test1", Name = "Test One", Email = "test1@example.com" },
                new UserDto { UserId = 2, Username = "test2", Name = "Test Two", Email = "test2@example.com" }
            };
            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _userService.GetAllUsersAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsUser()
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
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("test", result.Username);
        }

        [Fact]
        public async Task GetUserByUsername_ReturnsUserDto()
        {
            var dto = new UserDto
            {
                UserId = 1,
                Username = "john",
                Name = "John Doe",
                Email = "john@example.com"
            };
            _userRepositoryMock.Setup(r => r.GetUserByUsernameAsync("john")).ReturnsAsync(dto);

            var result = await _userService.GetUserByUsernameAsync("john");

            Assert.NotNull(result);
            Assert.Equal("john", result.Username);
        }

        [Fact]
        public async Task CreateUser_HashesPassword()
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

            await _userService.CreateUserAsync(user);

            _userRepositoryMock.Verify(r => r.CreateUserAsync(It.Is<User>(u =>
                u.Username == "john" &&
                !string.IsNullOrWhiteSpace(u.Password) &&
                u.Password != "1234"
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithPassword_HashesIt()
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

            await _userService.UpdateUserAsync(user);

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.Is<User>(u =>
                !string.IsNullOrWhiteSpace(u.Password) && u.Password != "secret123"
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithoutPassword_DoesNotHash()
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

            await _userService.UpdateUserAsync(user);

            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.Is<User>(u =>
                u.Password == ""
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateUserNameAndEmail_ValidId_UpdatesUser()
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
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            await _userService.UpdateUserNameAndEmailAsync(1, "New", "new@example.com");

            Assert.Equal("New", user.Name);
            Assert.Equal("new@example.com", user.Email);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserNameAndEmail_UserNotFound_ThrowsException()
        {
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(99)).ReturnsAsync((User?)null);

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await _userService.UpdateUserNameAndEmailAsync(99, "Test", "test@example.com"));

            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteUser_CallsRepository()
        {
            await _userService.DeleteUserAsync(5);

            _userRepositoryMock.Verify(r => r.DeleteUserAsync(5), Times.Once);
        }
    }
}
