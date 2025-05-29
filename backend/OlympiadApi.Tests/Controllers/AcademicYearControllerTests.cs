using Moq;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Controllers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Tests.Controllers
{
    public class AcademicYearControllerTests
    {
        private readonly Mock<IAcademicYearService> _serviceMock;
        private readonly AcademicYearController _controller;

        public AcademicYearControllerTests()
        {
            _serviceMock = new Mock<IAcademicYearService>();
            _controller = new AcademicYearController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAllAcademicYear_ReturnsOkResult_WithAcademicYears()
        {
            var academicYears = new List<AcademicYear>
            {
                new() { AcademicYearId = 1, StartYear = 2022, EndYear = 2023 },
                new() { AcademicYearId = 2, StartYear = 2023, EndYear = 2024 }
            };
            _serviceMock.Setup(s => s.GetAllAcademicYearsAsync()).ReturnsAsync(academicYears);

            var result = await _controller.GetAllAcademicYear();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<AcademicYear>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAllAcademicYear_ReturnsServerError_OnException()
        {
            _serviceMock.Setup(s => s.GetAllAcademicYearsAsync()).Throws(new Exception("Database failure"));

            var result = await _controller.GetAllAcademicYear();

            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public async Task GetAcademicYearById_ReturnsOkResult_WhenFound()
        {
            var year = new AcademicYear { AcademicYearId = 1, StartYear = 2022, EndYear = 2023 };
            _serviceMock.Setup(s => s.GetAcademicYearByIdAsync(1)).ReturnsAsync(year);

            var result = await _controller.GetAcademicYearById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AcademicYear>(okResult.Value);
            Assert.Equal(1, returnValue.AcademicYearId);
        }

        [Fact]
        public async Task GetAcademicYearById_ReturnsNotFound_WhenNotFound()
        {
            _serviceMock.Setup(s => s.GetAcademicYearByIdAsync(1)).ReturnsAsync((AcademicYear?)null);

            var result = await _controller.GetAcademicYearById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAcademicYearById_ReturnsServerError_OnException()
        {
            _serviceMock.Setup(s => s.GetAcademicYearByIdAsync(It.IsAny<int>())).Throws(new Exception("Unexpected error"));

            var result = await _controller.GetAcademicYearById(1);

            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }

        [Fact]
        public async Task CreateAcademicYear_ReturnsCreatedResult()
        {
            var newYear = new AcademicYear { AcademicYearId = 3, StartYear = 2024, EndYear = 2025 };

            var result = await _controller.CreateAcademicYear(newYear);

            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<AcademicYear>(createdAtAction.Value);
            Assert.Equal(newYear.AcademicYearId, returnValue.AcademicYearId);
        }

        [Fact]
        public async Task CreateAcademicYear_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.CreateAcademicYear(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateAcademicYear_ReturnsServerError_OnException()
        {
            var year = new AcademicYear { AcademicYearId = 4, StartYear = 2025, EndYear = 2026 };
            _serviceMock.Setup(s => s.AddAcademicYearAsync(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("DB Error"));

            var result = await _controller.CreateAcademicYear(year);

            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
        }
    }
}
