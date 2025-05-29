using Moq;
using Microsoft.Extensions.Configuration;
using OlympiadApi.Helpers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using System.Security.Claims;

namespace OlympiadApi.Tests.Helpers
{
    [Collection("NonParallelJwtTests")]
    public class JwtHelperTests
    {
        public JwtHelperTests()
        {
            var envPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "OlympiadApi", ".env"));
            DotNetEnv.Env.Load(envPath);
        }

        private JwtHelper CreateHelper(User? user = null, Dictionary<string, string?>? overrides = null)
        {
            var configData = new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY"),
                ["JWT_ISSUER"] = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                ["JWT_AUDIENCE"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                ["JWT_EXPIRATION_MINUTES"] = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES")
            };

            if (overrides != null)
            {
                foreach (var kvp in overrides)
                    configData[kvp.Key] = kvp.Value;
            }

            var config = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

            return new JwtHelper(config, userService.Object);
        }

        private User CreateTestUser(string? password = null) => new()
        {
            UserId = 1,
            Name = "Test User",
            DateOfBirth = new DateTime(2000, 1, 1),
            AcademicYearId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = password ?? BCrypt.Net.BCrypt.HashPassword("default123"),
            PersonalSettings = new(),
            Notifications = new(),
            CreatedAt = DateTime.UtcNow
        };

        [Fact]
        public void GenerateJwtToken_ReturnsValidToken()
        {
            var jwt = CreateHelper();
            var user = CreateTestUser();
            var roles = new Dictionary<string, Dictionary<string, bool>> { { "Admin", new() { { "Read", true } } } };

            var token = jwt.GenerateJwtToken(user, roles);

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Equal(2, token.Count(c => c == '.'));
        }

        [Theory]
        [InlineData("JWT_SECRET_KEY")]
        [InlineData("JWT_ISSUER")]
        [InlineData("JWT_AUDIENCE")]
        public void GenerateJwtToken_Throws_WhenRequiredEnvMissing(string missingKey)
        {
            var overrides = new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "supersecret",
                ["JWT_ISSUER"] = "issuer",
                ["JWT_AUDIENCE"] = "audience",
                ["JWT_EXPIRATION_MINUTES"] = "45"
            };

            overrides[missingKey] = null;

            var jwt = CreateHelper(CreateTestUser(), overrides);
            var ex = Assert.Throws<InvalidOperationException>(() => jwt.GenerateJwtToken(CreateTestUser(), new()));

            Assert.Contains(missingKey, ex.Message);
        }

        [Fact]
        public void GenerateJwtToken_Throws_WhenExpirationIsInvalid()
        {
            var overrides = new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "supersecret",
                ["JWT_ISSUER"] = "issuer",
                ["JWT_AUDIENCE"] = "audience",
                ["JWT_EXPIRATION_MINUTES"] = "invalid"
            };

            var jwt = CreateHelper(CreateTestUser(), overrides);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.GenerateJwtToken(CreateTestUser(), new()));
            Assert.Contains("JWT_EXPIRATION_MINUTES", ex.Message);
        }

        [Fact]
        public void GenerateJwtToken_Throws_WhenIssuerMissing()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "secret",
                ["JWT_AUDIENCE"] = "audience",
                ["JWT_EXPIRATION_MINUTES"] = "45"
            }).Build();

            var userService = new Mock<IUserService>();
            var jwt = new JwtHelper(config, userService.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.GenerateJwtToken(CreateTestUser(), new()));
            Assert.Contains("JWT_ISSUER", ex.Message);
        }

        [Fact]
        public void GenerateJwtToken_Throws_WhenAudienceMissing()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "secret",
                ["JWT_ISSUER"] = "issuer",
                ["JWT_EXPIRATION_MINUTES"] = "45"
            }).Build();

            var userService = new Mock<IUserService>();
            var jwt = new JwtHelper(config, userService.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.GenerateJwtToken(CreateTestUser(), new()));
            Assert.Contains("JWT_AUDIENCE", ex.Message);
        }

        [Fact]
        public void GenerateJwtToken_Throws_WhenExpirationInvalid()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "secret",
                ["JWT_ISSUER"] = "issuer",
                ["JWT_AUDIENCE"] = "audience",
                ["JWT_EXPIRATION_MINUTES"] = "invalid"
            }).Build();

            var userService = new Mock<IUserService>();
            var jwt = new JwtHelper(config, userService.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.GenerateJwtToken(CreateTestUser(), new()));
            Assert.Contains("JWT_EXPIRATION_MINUTES", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateJwtToken_Throws_WhenExpirationMissingOrWhitespace(string? expiryValue)
        {
            var overrides = new Dictionary<string, string?>
            {
                ["JWT_SECRET_KEY"] = "secret",
                ["JWT_ISSUER"] = "issuer",
                ["JWT_AUDIENCE"] = "audience",
                ["JWT_EXPIRATION_MINUTES"] = expiryValue
            };

            var jwt = CreateHelper(CreateTestUser(), overrides);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.GenerateJwtToken(CreateTestUser(), new()));
            Assert.Contains("JWT_EXPIRATION_MINUTES", ex.Message);
        }


        [Fact]
        public void ValidateJwtToken_ReturnsTrue_ForValidToken()
        {
            var jwt = CreateHelper();
            var token = jwt.GenerateJwtToken(CreateTestUser(), new());
            Assert.True(jwt.ValidateJwtToken(token));
        }

        [Fact]
        public void ValidateJwtToken_ReturnsFalse_ForInvalidToken()
        {
            var jwt = CreateHelper();
            var result = jwt.ValidateJwtToken("this.is.not.valid.jwt");
            Assert.False(result);
        }

        [Fact]
        public void ValidateJwtToken_Throws_WhenSecretKeyMissing()
        {
            var overrides = new Dictionary<string, string?>
            {
                ["JWT_ISSUER"] = "issuer",
                ["JWT_AUDIENCE"] = "audience"
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(overrides!)
                .Build();

            var userService = new Mock<IUserService>();
            var jwt = new JwtHelper(config, userService.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.ValidateJwtToken("token"));
            Assert.Contains("JWT_SECRET_KEY", ex.Message);
        }

        [Fact]
        public void ValidateJwtToken_Throws_WhenSecretMissing()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_ISSUER"] = "issuer",
                ["JWT_AUDIENCE"] = "audience"
            }).Build();

            var userService = new Mock<IUserService>();
            var jwt = new JwtHelper(config, userService.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => jwt.ValidateJwtToken("any"));
            Assert.Contains("JWT_SECRET_KEY", ex.Message);
        }

        [Fact]
        public void GetClaimsFromJwt_ParsesClaimsSuccessfully()
        {
            var jwt = CreateHelper();
            var user = CreateTestUser();
            var token = jwt.GenerateJwtToken(user, new());
            var claims = jwt.GetClaimsFromJwt(token);

            Assert.NotNull(claims);
            Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == user.Username);
        }

        [Fact]
        public void GetClaimsFromJwt_ReturnsNull_WhenCannotRead()
        {
            var jwt = CreateHelper();
            var result = jwt.GetClaimsFromJwt("!!!bad-token");

            Assert.Null(result);
        }

        [Fact]
        public void GetClaimsFromJwt_ReturnsNull_ForUnreadableToken()
        {
            var jwt = CreateHelper();
            Assert.Null(jwt.GetClaimsFromJwt("invalid-token"));
        }

        [Fact]
        public void GetClaimsFromJwt_ReturnsNull_WhenJwtTokenIsNull()
        {
            var jwt = CreateHelper();
            var claims = jwt.GetClaimsFromJwt(null!);

            Assert.Null(claims);
        }


        [Fact]
        public async Task ValidatePassword_ReturnsTrue_WhenCorrect()
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword("secret123");
            var user = CreateTestUser(hashed);

            var jwt = CreateHelper(user);
            Assert.True(await jwt.ValidatePasswordAsync(1, "secret123"));
        }

        [Fact]
        public async Task ValidatePassword_ReturnsFalse_WhenIncorrect()
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword("secret123");
            var user = CreateTestUser(hashed);

            var jwt = CreateHelper(user);
            Assert.False(await jwt.ValidatePasswordAsync(1, "wrong"));
        }

        [Fact]
        public async Task ValidatePassword_ReturnsFalse_WhenUserNotFound()
        {
            var jwt = CreateHelper(user: null);
            Assert.False(await jwt.ValidatePasswordAsync(42, "any"));
        }
    }
}