using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlympiadApi.Tests.Services
{
    public class UserRoleAssignmentServiceTests
    {
        private readonly Mock<IUserRoleAssignmentRepository> _repositoryMock;
        private readonly Mock<ILogger<UserRoleAssignmentService>> _loggerMock;
        private readonly UserRoleAssignmentService _service;

        public UserRoleAssignmentServiceTests()
        {
            _repositoryMock = new Mock<IUserRoleAssignmentRepository>();
            _loggerMock = new Mock<ILogger<UserRoleAssignmentService>>();
            _service = new UserRoleAssignmentService(_repositoryMock.Object, _loggerMock.Object);
        }

        private UserRoleAssignment CreateValidAssignment() => new()
        {
            AssignmentId = 1,
            UserId = 100,
            RoleId = 200,
            AssignedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task GetAllAssignmentsAsync_ReturnsAssignments()
        {
            var list = new List<UserRoleAssignment> { CreateValidAssignment() };
            _repositoryMock.Setup(r => r.GetAllAssignmentsAsync()).ReturnsAsync(list);

            var result = await _service.GetAllAssignmentsAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllAssignmentsAsync_ThrowsException_LogsError()
        {
            _repositoryMock.Setup(r => r.GetAllAssignmentsAsync()).ThrowsAsync(new Exception("DB error"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllAssignmentsAsync());

            Assert.Contains("retrieving", ex.Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString() != null && v.ToString()!.Contains("Error retrieving user role assignments")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ReturnsAssignment()
        {
            var assignment = CreateValidAssignment();
            _repositoryMock.Setup(r => r.GetAssignmentByIdAsync(1)).ReturnsAsync(assignment);

            var result = await _service.GetAssignmentByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(100, result.UserId);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ThrowsException_LogsError()
        {
            _repositoryMock.Setup(r => r.GetAssignmentByIdAsync(99)).ThrowsAsync(new Exception("not found"));

            await Assert.ThrowsAsync<Exception>(() => _service.GetAssignmentByIdAsync(99));

            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString() != null && v.ToString()!.Contains("Error retrieving user role assignment")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAssignmentAsync_ValidAssignment_ReturnsAssignment()
        {
            var assignment = CreateValidAssignment();
            _repositoryMock.Setup(r => r.GetAllAssignmentsAsync()).ReturnsAsync(new List<UserRoleAssignment>());
            _repositoryMock.Setup(r => r.CreateAssignmentAsync(assignment)).ReturnsAsync(assignment);

            var result = await _service.CreateAssignmentAsync(assignment);

            Assert.NotNull(result);
            Assert.Equal(assignment, result);
        }

        [Fact]
        public async Task CreateAssignmentAsync_NullAssignment_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAssignmentAsync(null!));
        }

        [Fact]
        public async Task CreateAssignmentAsync_NoUserOrRole_ThrowsExceptionAndLogs()
        {
            var assignment = CreateValidAssignment();
            _repositoryMock.Setup(r => r.GetAllAssignmentsAsync()).ReturnsAsync((List<UserRoleAssignment>?)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAssignmentAsync(assignment));

            Assert.Contains("creating", ex.Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString() != null && v.ToString()!.Contains("Error creating user role assignment")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ReturnsTrue()
        {
            _repositoryMock.Setup(r => r.DeleteAssignmentAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAssignmentAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ThrowsException_LogsError()
        {
            _repositoryMock.Setup(r => r.DeleteAssignmentAsync(1)).ThrowsAsync(new Exception("delete fail"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteAssignmentAsync(1));

            Assert.Contains("deleting", ex.Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString() != null && v.ToString()!.Contains("Error deleting user role assignment")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
