using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Controllers;
using OlympiadApi.DTOs;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.Helpers;
using System.Text.Json;

namespace OlympiadApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        private Dictionary<string, object> AsDictionary(object? value)
        {
            var json = JsonSerializer.Serialize(value);
            return JsonSerializationHelper.DeserializeFromJson(json)!;
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginDto>())).ReturnsAsync(new { token = "abc123" });

            var result = await _controller.Login(new LoginDto { UsernameOrEmail = "user", Password = "pass" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(okResult.Value);
            Assert.Equal("abc123", dict["token"]?.ToString());
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginDto>())).ReturnsAsync((object?)null);

            var result = await _controller.Login(new LoginDto { UsernameOrEmail = "user", Password = "wrong" });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Invalid username or password.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task RequestPasswordChange_ReturnsOk_WhenSuccessful()
        {
            _authServiceMock.Setup(s => s.RequestPasswordChangeAsync(It.IsAny<PasswordChangeRequestDto>())).ReturnsAsync(true);

            var result = await _controller.RequestPasswordChange(new PasswordChangeRequestDto { UsernameOrEmail = "user" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(ok.Value);
            Assert.Equal("Password reset instructions sent to your email.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task RequestPasswordChange_ReturnsNotFound_WhenUserNotFound()
        {
            _authServiceMock.Setup(s => s.RequestPasswordChangeAsync(It.IsAny<PasswordChangeRequestDto>())).ReturnsAsync(false);

            var result = await _controller.RequestPasswordChange(new PasswordChangeRequestDto { UsernameOrEmail = "invalid" });

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var dict = AsDictionary(notFound.Value);
            Assert.Equal("User not found.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenTokenValid()
        {
            _authServiceMock.Setup(s => s.ResetPasswordAsync("token123", It.IsAny<ResetPasswordDto>())).ReturnsAsync(true);

            var result = await _controller.ResetPasswordAsync(new ResetPasswordDto { NewPassword = "newpass" }, "token123");

            var ok = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(ok.Value);
            Assert.Equal("Password updated successfully.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenTokenInvalid()
        {
            _authServiceMock.Setup(s => s.ResetPasswordAsync("invalid", It.IsAny<ResetPasswordDto>())).ReturnsAsync(false);

            var result = await _controller.ResetPasswordAsync(new ResetPasswordDto { NewPassword = "pass" }, "invalid");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var dict = AsDictionary(badRequest.Value);
            Assert.Equal("Invalid or expired reset token.", dict["message"]?.ToString());
        }

        [Fact]
        public void ValidateToken_ReturnsOk_WhenTokenValid()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Bearer valid-token";
            _authServiceMock.Setup(s => s.ValidateToken("valid-token")).Returns(true);

            var result = _controller.ValidateToken();

            var ok = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(ok.Value);
            Assert.Equal("Token is valid.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsOk_WhenPasswordValid()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Bearer token";

            _authServiceMock.Setup(s => s.ValidatePasswordAsync("token", It.IsAny<ValidatePasswordDto>())).ReturnsAsync((true, "Password validated successfully."));

            var result = await _controller.ValidatePasswordAsync(new ValidatePasswordDto { Password = "pass" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var dict = AsDictionary(ok.Value);
            Assert.Equal("Password validated successfully.", dict["message"]?.ToString());
        }

        [Fact]
        public void ValidateToken_ReturnsUnauthorized_WhenTokenMissing()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = _controller.ValidateToken();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Token is missing or invalid.", dict["message"]?.ToString());
        }

        [Fact]
        public void ValidateToken_ReturnsUnauthorized_WhenTokenMalformed()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Token abc";

            var result = _controller.ValidateToken();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Token is missing or invalid.", dict["message"]?.ToString());
        }

        [Fact]
        public void ValidateToken_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Bearer invalid-token";

            _authServiceMock.Setup(s => s.ValidateToken("invalid-token")).Returns(false);

            var result = _controller.ValidateToken();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Token is invalid or expired.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsUnauthorized_WhenTokenMalformed()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Token xyz";

            var result = await _controller.ValidatePasswordAsync(new ValidatePasswordDto { Password = "pass" });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Token is missing or invalid.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsUnauthorized_WhenAuthHeaderWhitespaceOnly()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.Request.Headers["Authorization"] = "     ";

            var result = await _controller.ValidatePasswordAsync(new ValidatePasswordDto { Password = "pass" });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Token is missing or invalid.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsUnauthorized_WhenPasswordInvalid()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Bearer token";

            _authServiceMock.Setup(s => s.ValidatePasswordAsync("token", It.IsAny<ValidatePasswordDto>())).ReturnsAsync((false, "Invalid password."));

            var result = await _controller.ValidatePasswordAsync(new ValidatePasswordDto { Password = "bad" });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Invalid password.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsUnauthorized_WhenTokenMissingBearerPrefix()
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            _controller.Request.Headers["Authorization"] = "Basic token123";

            var result = await _controller.ValidatePasswordAsync(new ValidatePasswordDto { Password = "pass" });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var dict = AsDictionary(unauthorized.Value);
            Assert.Equal("Token is missing or invalid.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsBadRequest_WhenMissingPassword()
        {
            var result = await _controller.ValidatePasswordAsync(new ValidatePasswordDto { Password = "" });

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var dict = AsDictionary(badRequest.Value);
            Assert.Equal("Password is required.", dict["message"]?.ToString());
        }

        [Fact]
        public async Task ValidatePassword_ReturnsBadRequest_WhenDtoIsNull()
        {
            var result = await _controller.ValidatePasswordAsync(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var dict = AsDictionary(badRequest.Value);
            Assert.Equal("Password is required.", dict["message"]?.ToString());
        }
    }
}
