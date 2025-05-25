using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OlympiadApi.Helpers;
using OlympiadApi.Services.Interfaces;
using Moq;
using Microsoft.AspNetCore.Hosting;

namespace OlympiadApi.Tests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly Dictionary<string, string?> _envVars = new();

        public void SetEnvironmentVariable(string key, string? value)
        {
            _envVars[key] = value;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(_envVars!);
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IJwtHelper>();
                services.RemoveAll<IUserService>();
                services.RemoveAll<IEmailService>();
                services.RemoveAll<ISmtpClient>();

                var jwtMock = new Mock<IJwtHelper>();
                jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(true);
                jwtMock.Setup(j => j.GetClaimsFromJwt(It.IsAny<string>()))
                    .Returns([new System.Security.Claims.Claim("Roles", "Admin")]);

                var userServiceMock = new Mock<IUserService>();
                userServiceMock.Setup(u => u.GetUserById(It.IsAny<int>())).Returns(new OlympiadApi.Models.User
                {
                    UserId = 1,
                    Name = "Test",
                    Username = "test",
                    Email = "test@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    AcademicYearId = 1,
                    DateOfBirth = DateTime.Today
                });

                var emailServiceMock = new Mock<IEmailService>();

                services.AddScoped(_ => jwtMock.Object);
                services.AddScoped(_ => userServiceMock.Object);
                services.AddScoped(_ => emailServiceMock.Object);
            });
        }
    }
}
