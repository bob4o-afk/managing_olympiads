using Moq;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Controllers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using System.Threading.Tasks;

namespace OlympiadApi.Tests.Controllers
{
    public class OlympiadControllerTests
    {
        private readonly Mock<IOlympiadService> _serviceMock;
        private readonly OlympiadController _controller;

        public OlympiadControllerTests()
        {
            _serviceMock = new Mock<IOlympiadService>();
            _controller = new OlympiadController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAllOlympiads_ReturnsOk_WithOlympiads()
        {
            var olympiads = new List<Olympiad>
            {
                new() {
                    OlympiadId = 1,
                    Subject = "Math",
                    Description = "Math Olympiad",
                    DateOfOlympiad = new DateTime(2024, 11, 1),
                    Round = "Regional",
                    Location = "Sofia",
                    ClassNumber = 10
                },
                new() {
                    OlympiadId = 2,
                    Subject = "Physics",
                    Description = "Physics Olympiad",
                    DateOfOlympiad = new DateTime(2024, 12, 10),
                    Round = "National",
                    Location = "Plovdiv",
                    ClassNumber = 12
                }
            };

            _serviceMock.Setup(s => s.GetAllOlympiadsAsync()).ReturnsAsync(olympiads);

            var result = await _controller.GetAllOlympiads();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Olympiad>>(okResult.Value);
            Assert.Equal(2, ((List<Olympiad>)returnValue).Count);
        }

        [Fact]
        public async Task GetAllOlympiads_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetAllOlympiadsAsync()).Throws(new Exception("DB error"));

            var result = await _controller.GetAllOlympiads();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetOlympiadById_ReturnsOk_WhenFound()
        {
            var olympiad = new Olympiad
            {
                OlympiadId = 1,
                Subject = "Chemistry",
                Description = "Chem Olympiad",
                DateOfOlympiad = new DateTime(2024, 12, 1),
                Round = "District",
                Location = "Varna",
                ClassNumber = 9
            };
            _serviceMock.Setup(s => s.GetOlympiadByIdAsync(1)).ReturnsAsync(olympiad);

            var result = await _controller.GetOlympiadById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOlympiad = Assert.IsType<Olympiad>(okResult.Value);
            Assert.Equal(1, returnedOlympiad.OlympiadId);
        }

        [Fact]
        public async Task GetOlympiadById_ReturnsNotFound_WhenMissing()
        {
            _serviceMock.Setup(s => s.GetOlympiadByIdAsync(1)).ReturnsAsync((Olympiad?)null);

            var result = await _controller.GetOlympiadById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetOlympiadById_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetOlympiadByIdAsync(It.IsAny<int>())).Throws(new Exception("Something went wrong"));

            var result = await _controller.GetOlympiadById(2);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateOlympiad_ReturnsCreated_WhenValid()
        {
            var olympiad = new Olympiad
            {
                OlympiadId = 5,
                Subject = "Biology",
                Description = "Bio Olympiad",
                DateOfOlympiad = new DateTime(2024, 10, 5),
                Round = "School",
                Location = "Burgas",
                StartTime = DateTime.Now,
                ClassNumber = 8
            };

            _serviceMock.Setup(s => s.AddOlympiadAsync(It.IsAny<Olympiad>())).Returns((Olympiad o) => Task.FromResult(o));

            var result = await _controller.CreateOlympiad(olympiad);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(olympiad, created.Value);
            Assert.Equal("GetOlympiadById", created.ActionName);
            Assert.Equal(olympiad.OlympiadId, created.RouteValues!["id"]);
        }

        [Fact]
        public async Task CreateOlympiad_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.CreateOlympiad(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Invalid data", badRequest.Value!.ToString());
        }

        [Fact]
        public async Task CreateOlympiad_Returns500_OnException()
        {
            var olympiad = new Olympiad
            {
                OlympiadId = 1,
                Subject = "Error",
                Description = "Error",
                DateOfOlympiad = DateTime.Now,
                Round = "Error",
                Location = "Error",
                StartTime = DateTime.Now,
                ClassNumber = 11
            };

            _serviceMock.Setup(s => s.AddOlympiadAsync(It.IsAny<Olympiad>())).Throws(new Exception("Insert error"));

            var result = await _controller.CreateOlympiad(olympiad);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }
    }
}
