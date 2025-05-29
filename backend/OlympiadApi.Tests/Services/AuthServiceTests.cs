using Moq;
using OlympiadApi.Services;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Helpers;
using OlympiadApi.DTOs;
using OlympiadApi.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace OlympiadApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IJwtHelper> _jwtHelperMock;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _authRepoMock = new Mock<IAuthRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _configMock = new Mock<IConfiguration>();
            _jwtHelperMock = new Mock<IJwtHelper>();

            _authService = new AuthService(
                _authRepoMock.Object,
                _jwtHelperMock.Object,
                _emailServiceMock.Object,
                _configMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenAndUser()
        {
            var dto = new LoginDto { UsernameOrEmail = "user", Password = "pass" };
            var userDto = new UserDto
            {
                UserId = 1,
                Email = "user@test.com",
                Username = "user",
                Name = "User"
            };
            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                DateOfBirth = DateTime.UtcNow.AddYears(-18),
                AcademicYearId = 1,
                Username = "testuser",
                Email = "user@test.com",
                Password = "secret",
                AcademicYear = new AcademicYear
                {
                    AcademicYearId = 1,
                    StartYear = 2024,
                    EndYear = 2025
                }
            };

            var roles = new Dictionary<string, Dictionary<string, bool>>
            {
                { "Admin", new Dictionary<string, bool> { { "Read", true } } }
            };

            _authRepoMock.Setup(x => x.AuthenticateUserAsync(dto.UsernameOrEmail, dto.Password)).ReturnsAsync(userDto);
            _authRepoMock.Setup(x => x.GetUserRolesWithPermissionsAsync(userDto.UserId)).ReturnsAsync(roles);
            _userRepoMock.Setup(x => x.FindUserByUsernameOrEmailAsync(userDto.Email)).ReturnsAsync(user);
            _jwtHelperMock.Setup(x => x.GenerateJwtToken(user, roles)).Returns("fake-jwt-token");

            var result = await _authService.LoginAsync(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            _authRepoMock.Setup(x => x.AuthenticateUserAsync("baduser", "badpass")).ReturnsAsync((UserDto?)null);

            var result = await _authService.LoginAsync(new LoginDto { UsernameOrEmail = "baduser", Password = "badpass" });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenUserNotFoundInUserRepository()
        {
            var loginDto = new LoginDto
            {
                UsernameOrEmail = "user@example.com",
                Password = "pass"
            };

            var userDto = new UserDto
            {
                UserId = 1,
                Email = "user@example.com",
                Name = "Test User",
                Username = "testuser"
            };

            var fakeRoles = new Dictionary<string, Dictionary<string, bool>>
            {
                { "Admin", new Dictionary<string, bool> { { "Read", true } } }
            };

            _authRepoMock
                .Setup(x => x.AuthenticateUserAsync(loginDto.UsernameOrEmail, loginDto.Password))
                .ReturnsAsync(userDto);

            _authRepoMock
                .Setup(x => x.GetUserRolesWithPermissionsAsync(userDto.UserId))
                .ReturnsAsync(fakeRoles);

            _userRepoMock
                .Setup(x => x.FindUserByUsernameOrEmailAsync(userDto.Email))
                .ReturnsAsync((User?)null);

            var result = await _authService.LoginAsync(loginDto);

            Assert.Null(result);
        }


        [Fact]
        public async Task RequestPasswordChange_ValidUser_SendsEmail()
        {
            var request = new PasswordChangeRequestDto { UsernameOrEmail = "user" };
            var userDto = new UserDto
            {
                UserId = 1,
                Name = "User",
                Username = "user",
                Email = "user@test.com"
            };

            _authRepoMock.Setup(r => r.GetUserByEmailOrUsernameAsync(request.UsernameOrEmail)).ReturnsAsync(userDto);
            _configMock.Setup(c => c["FrontendUrl"]).Returns("http://frontend");

            var result = await _authService.RequestPasswordChangeAsync(request);

            Assert.True(result);
            _emailServiceMock.Verify(e => e.SendEmailAsync(
                userDto.Email,
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains("reset-password")),
                It.IsAny<string>()
            ), Times.Once);
        }

        [Fact]
        public async Task RequestPasswordChange_InvalidUser_ReturnsFalse()
        {
            _authRepoMock.Setup(r => r.GetUserByEmailOrUsernameAsync("missing")).ReturnsAsync((UserDto?)null);

            var result = await _authService.RequestPasswordChangeAsync(new PasswordChangeRequestDto { UsernameOrEmail = "missing" });

            Assert.False(result);
        }

        [Fact]
        public async Task ResetPassword_ValidToken_ReturnsTrue()
        {
            _authRepoMock.Setup(r => r.ValidateResetTokenAsync("valid")).ReturnsAsync(true);
            _authRepoMock.Setup(r => r.ResetPasswordWithTokenAsync("valid", "newpass")).ReturnsAsync(true);

            var result = await _authService.ResetPasswordAsync("valid", new ResetPasswordDto { NewPassword = "newpass" });

            Assert.True(result);
        }

        [Fact]
        public async Task ResetPassword_InvalidToken_ReturnsFalse()
        {
            _authRepoMock.Setup(r => r.ValidateResetTokenAsync("invalid")).ReturnsAsync(false);

            var result = await _authService.ResetPasswordAsync("invalid", new ResetPasswordDto { NewPassword = "pass" });

            Assert.False(result);
        }

        [Fact]
        public void ValidateToken_ValidToken_ReturnsTrue()
        {
            _jwtHelperMock.Setup(j => j.ValidateJwtToken("token")).Returns(true);

            Assert.True(_authService.ValidateToken("token"));
        }

        [Fact]
        public async Task ValidatePassword_ValidTokenAndPassword_ReturnsTrue()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            _jwtHelperMock.Setup(j => j.GetClaimsFromJwt("valid-token")).Returns(claims);
            _authRepoMock.Setup(r => r.ValidateUserPasswordAsync(1, "pass")).ReturnsAsync(true);

            var result = await _authService.ValidatePasswordAsync("valid-token", new ValidatePasswordDto { Password = "pass" });

            Assert.True(result.IsValid);
            Assert.Equal("Password validated successfully.", result.Message);
        }

        [Fact]
        public async Task ValidatePassword_InvalidToken_ReturnsFalse()
        {
            _jwtHelperMock.Setup(j => j.GetClaimsFromJwt("bad-token")).Returns((IEnumerable<Claim>?)null);

            var result = await _authService.ValidatePasswordAsync("bad-token", new ValidatePasswordDto { Password = "pass" });

            Assert.False(result.IsValid);
            Assert.Equal("Token is invalid.", result.Message);
        }

        [Fact]
        public async Task ValidatePassword_InvalidPassword_ReturnsFalse()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            _jwtHelperMock.Setup(j => j.GetClaimsFromJwt("valid-token")).Returns(claims);
            _authRepoMock.Setup(r => r.ValidateUserPasswordAsync(1, "wrong")).ReturnsAsync(false);

            var result = await _authService.ValidatePasswordAsync("valid-token", new ValidatePasswordDto { Password = "wrong" });

            Assert.False(result.IsValid);
            Assert.Equal("Invalid password.", result.Message);
        }

        [Fact]
        public async Task ValidatePassword_InvalidUserIdFormat_ReturnsFalse()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "not-an-integer")
            };

            _jwtHelperMock.Setup(j => j.GetClaimsFromJwt("token-bad-id")).Returns(claims);

            var result = await _authService.ValidatePasswordAsync("token-bad-id", new ValidatePasswordDto { Password = "pass" });

            Assert.False(result.IsValid);
            Assert.Equal("User ID claim not found in token.", result.Message);
        }


        [Fact]
        public async Task ValidatePassword_MissingUserIdClaim_ReturnsFalse()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "user@test.com")
            };

            _jwtHelperMock.Setup(j => j.GetClaimsFromJwt("token-no-id")).Returns(claims);

            var result = await _authService.ValidatePasswordAsync("token-no-id", new ValidatePasswordDto { Password = "pass" });

            Assert.False(result.IsValid);
            Assert.Equal("User ID claim not found in token.", result.Message);
        }

    }
}
