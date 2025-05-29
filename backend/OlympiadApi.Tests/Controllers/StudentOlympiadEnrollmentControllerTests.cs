using Moq;
using OlympiadApi.Controllers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OlympiadApi.Tests.Controllers
{
    public class StudentOlympiadEnrollmentControllerTests
    {
        private readonly Mock<IStudentOlympiadEnrollmentService> _mockService;
        private readonly StudentOlympiadEnrollmentController _controller;

        public StudentOlympiadEnrollmentControllerTests()
        {
            _mockService = new Mock<IStudentOlympiadEnrollmentService>();
            _controller = new StudentOlympiadEnrollmentController(_mockService.Object);
        }

        private StudentOlympiadEnrollment CreateValidEnrollment(int id = 1)
        {
            return new StudentOlympiadEnrollment
            {
                EnrollmentId = id,
                UserId = 100,
                OlympiadId = 200,
                AcademicYearId = 300,
                EnrollmentStatus = "Registered",
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetAllEnrollments_ReturnsOk()
        {
            var list = new List<StudentOlympiadEnrollment> { CreateValidEnrollment() };
            _mockService.Setup(s => s.GetAllEnrollmentsAsync()).ReturnsAsync(list);

            var result = await _controller.GetAllEnrollments();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetEnrollmentById_ReturnsOk()
        {
            var enrollment = CreateValidEnrollment();
            _mockService.Setup(s => s.GetEnrollmentByIdAsync(1)).ReturnsAsync(enrollment);

            var result = await _controller.GetEnrollmentById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(enrollment, ok.Value);
        }

        [Fact]
        public async Task GetEnrollmentById_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetEnrollmentByIdAsync(1)).ReturnsAsync((StudentOlympiadEnrollment?)null);

            var result = await _controller.GetEnrollmentById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetEnrollmentsByUserId_ReturnsOk()
        {
            var list = new List<StudentOlympiadEnrollment> { CreateValidEnrollment() };
            _mockService.Setup(s => s.GetEnrollmentsByUserIdAsync(5)).ReturnsAsync(list);

            var result = await _controller.GetEnrollmentsByUserId(5);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetEnrollmentsByUserId_ReturnsNotFound_WhenEnrollmentsIsNull()
        {
            _mockService.Setup(s => s.GetEnrollmentsByUserIdAsync(5)).ReturnsAsync((List<StudentOlympiadEnrollment>?)null!);

            var result = await _controller.GetEnrollmentsByUserId(5);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonSerializer.Serialize(notFound.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            Assert.Equal("No enrollments found for this user.", dict["Message"]?.ToString());
        }

        [Fact]
        public async Task GetEnrollmentsByUserId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetEnrollmentsByUserIdAsync(5)).ReturnsAsync(new List<StudentOlympiadEnrollment>());

            var result = await _controller.GetEnrollmentsByUserId(5);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            var json = JsonSerializer.Serialize(notFound.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            Assert.Equal("No enrollments found for this user.", dict["Message"]?.ToString());
        }

        [Fact]
        public async Task CreateEnrollment_ReturnsCreated()
        {
            var enrollment = CreateValidEnrollment(10);
            _mockService.Setup(s => s.CreateEnrollmentAsync(enrollment)).ReturnsAsync(enrollment);

            var result = await _controller.CreateEnrollment(enrollment);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(enrollment, created.Value);
            Assert.Equal(nameof(_controller.GetEnrollmentById), created.ActionName);
        }

        [Fact]
        public async Task CreateEnrollment_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Error", "Invalid");

            var enrollment = CreateValidEnrollment();

            var result = await _controller.CreateEnrollment(enrollment);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEnrollment_ReturnsOk()
        {
            var enrollment = CreateValidEnrollment(2);
            _mockService.Setup(s => s.UpdateEnrollmentAsync(2, enrollment)).ReturnsAsync(enrollment);

            var result = await _controller.UpdateEnrollment(2, enrollment);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(enrollment, ok.Value);
        }

        [Fact]
        public async Task UpdateEnrollment_ReturnsNotFound()
        {
            var enrollment = CreateValidEnrollment(2);
            _mockService.Setup(s => s.UpdateEnrollmentAsync(2, enrollment)).ReturnsAsync((StudentOlympiadEnrollment?)null);

            var result = await _controller.UpdateEnrollment(2, enrollment);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateEnrollment_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Error", "Invalid");

            var enrollment = CreateValidEnrollment(2);

            var result = await _controller.UpdateEnrollment(2, enrollment);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEnrollment_IdMismatch_ReturnsNotFound()
        {
            var enrollment = CreateValidEnrollment(99);

            var result = await _controller.UpdateEnrollment(2, enrollment);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateEnrollment_InvalidModelAndIdMatch_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Test", "Invalid");

            var enrollment = CreateValidEnrollment(2);

            var result = await _controller.UpdateEnrollment(2, enrollment);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEnrollment_ReturnsNoContent()
        {
            _mockService.Setup(s => s.DeleteEnrollmentAsync(3)).ReturnsAsync(true);

            var result = await _controller.DeleteEnrollment(3);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteEnrollment_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteEnrollmentAsync(3)).ReturnsAsync(false);

            var result = await _controller.DeleteEnrollment(3);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllEnrollments_Throws_Returns500()
        {
            _mockService.Setup(s => s.GetAllEnrollmentsAsync()).ThrowsAsync(new Exception("Database error"));

            var result = await _controller.GetAllEnrollments();

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetEnrollmentById_Throws_Returns500()
        {
            _mockService.Setup(s => s.GetEnrollmentByIdAsync(1)).ThrowsAsync(new Exception("Failure"));

            var result = await _controller.GetEnrollmentById(1);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetEnrollmentsByUserId_Throws_Returns500()
        {
            _mockService.Setup(s => s.GetEnrollmentsByUserIdAsync(5)).ThrowsAsync(new Exception("Failure"));

            var result = await _controller.GetEnrollmentsByUserId(5);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task CreateEnrollment_Throws_Returns500()
        {
            var enrollment = CreateValidEnrollment(10);
            _mockService.Setup(s => s.CreateEnrollmentAsync(enrollment)).ThrowsAsync(new Exception("Error"));

            var result = await _controller.CreateEnrollment(enrollment);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task UpdateEnrollment_Throws_Returns500()
        {
            var enrollment = CreateValidEnrollment(2);
            _mockService.Setup(s => s.UpdateEnrollmentAsync(2, enrollment)).ThrowsAsync(new Exception("Failure"));

            var result = await _controller.UpdateEnrollment(2, enrollment);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task DeleteEnrollment_Throws_Returns500()
        {
            _mockService.Setup(s => s.DeleteEnrollmentAsync(3)).ThrowsAsync(new Exception("Failure"));

            var result = await _controller.DeleteEnrollment(3);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }
    }
}
