using Moq;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Controllers;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Tests.Controllers
{
    public class UserRoleAssignmentControllerTests
    {
        private readonly Mock<IUserRoleAssignmentService> _mockService;
        private readonly UserRoleAssignmentController _controller;

        public UserRoleAssignmentControllerTests()
        {
            _mockService = new Mock<IUserRoleAssignmentService>();
            _controller = new UserRoleAssignmentController(_mockService.Object);
        }

        private UserRoleAssignment CreateSampleAssignment(int id = 1) => new UserRoleAssignment
        {
            AssignmentId = id,
            UserId = 1,
            RoleId = 2,
            AssignedAt = new DateTime(2024, 1, 1),
            User = null,
            Role = null
        };

        [Fact]
        public async Task GetAllAssignments_ReturnsOk_WithList()
        {
            var assignments = new List<UserRoleAssignment> { CreateSampleAssignment() };
            _mockService.Setup(s => s.GetAllAssignmentsAsync()).ReturnsAsync(assignments);

            var result = await _controller.GetAllAssignments();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(assignments, ok.Value);
        }

        [Fact]
        public async Task GetAssignmentById_ReturnsOk_WhenFound()
        {
            var assignment = CreateSampleAssignment();
            _mockService.Setup(s => s.GetAssignmentByIdAsync(1)).ReturnsAsync(assignment);

            var result = await _controller.GetAssignmentById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(assignment, ok.Value);
        }

        [Fact]
        public async Task GetAssignmentById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetAssignmentByIdAsync(1)).ReturnsAsync((UserRoleAssignment?)null);

            var result = await _controller.GetAssignmentById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAssignment_ReturnsCreated_WhenValid()
        {
            var assignment = CreateSampleAssignment();
            _mockService.Setup(s => s.CreateAssignmentAsync(assignment)).ReturnsAsync(assignment);

            var result = await _controller.CreateAssignment(assignment);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(assignment, created.Value);
        }

        [Fact]
        public async Task CreateAssignment_ReturnsBadRequest_WhenModelInvalid()
        {
            _controller.ModelState.AddModelError("RoleId", "Required");

            var assignment = new UserRoleAssignment
            {
                UserId = 1,
                RoleId = 0, // this is made intentionally invalid cuz it's a good practice 
                AssignedAt = DateTime.UtcNow
            };

            var result = await _controller.CreateAssignment(assignment);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAssignment_ReturnsNoContent_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteAssignmentAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteAssignment(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAssignment_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.DeleteAssignmentAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeleteAssignment(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
