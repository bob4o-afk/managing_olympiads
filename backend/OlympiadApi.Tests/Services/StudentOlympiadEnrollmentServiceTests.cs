using Moq;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Tests.Services
{
    public class StudentOlympiadEnrollmentServiceTests
    {
        private readonly Mock<IStudentOlympiadEnrollmentRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IOlympiadRepository> _olympiadRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly StudentOlympiadEnrollmentService _service;

        public StudentOlympiadEnrollmentServiceTests()
        {
            _repositoryMock = new Mock<IStudentOlympiadEnrollmentRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _olympiadRepositoryMock = new Mock<IOlympiadRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _service = new StudentOlympiadEnrollmentService(_repositoryMock.Object, _userRepositoryMock.Object, _olympiadRepositoryMock.Object, _emailServiceMock.Object);
        }

        private StudentOlympiadEnrollment CreateValidEnrollment() => new StudentOlympiadEnrollment
        {
            EnrollmentId = 1,
            UserId = 1,
            OlympiadId = 1,
            AcademicYearId = 1,
            EnrollmentStatus = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        private StudentOlympiadEnrollment CloneEnrollment(StudentOlympiadEnrollment original, int? userId = null)
        {
            return new StudentOlympiadEnrollment
            {
                EnrollmentId = original.EnrollmentId,
                UserId = userId ?? original.UserId,
                OlympiadId = original.OlympiadId,
                AcademicYearId = original.AcademicYearId,
                EnrollmentStatus = original.EnrollmentStatus,
                CreatedAt = original.CreatedAt
            };
        }

        [Fact]
        public async Task GetAllEnrollmentsAsync_ReturnsAll()
        {
            var enrollments = new List<StudentOlympiadEnrollment> { CreateValidEnrollment(), CreateValidEnrollment() };
            _repositoryMock.Setup(r => r.GetAllEnrollmentsAsync()).ReturnsAsync(enrollments);

            var result = await _service.GetAllEnrollmentsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEnrollmentByIdAsync_ReturnsEnrollment()
        {
            var enrollment = CreateValidEnrollment();
            _repositoryMock.Setup(r => r.GetEnrollmentByIdAsync(1)).ReturnsAsync(enrollment);

            var result = await _service.GetEnrollmentByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.EnrollmentId);
        }

        [Fact]
        public async Task GetEnrollmentsByUserIdAsync_ReturnsEnrollments()
        {
            var enrollments = new List<StudentOlympiadEnrollment>
            {
                CloneEnrollment(CreateValidEnrollment(), userId: 2),
                CloneEnrollment(CreateValidEnrollment(), userId: 2)
            };
            _repositoryMock.Setup(r => r.GetEnrollmentsByUserIdAsync(2)).ReturnsAsync(enrollments);

            var result = await _service.GetEnrollmentsByUserIdAsync(2);

            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.Equal(2, e.UserId));
        }

        [Fact]
        public async Task CreateEnrollmentAsync_ValidEnrollment_CreatesEnrollment()
        {
            var enrollment = CreateValidEnrollment();
            _repositoryMock.Setup(r => r.GetAllEnrollmentsAsync()).ReturnsAsync(new List<StudentOlympiadEnrollment>());
            _repositoryMock.Setup(r => r.CreateEnrollmentAsync(enrollment)).ReturnsAsync(enrollment);

            var result = await _service.CreateEnrollmentAsync(enrollment);

            Assert.Equal(enrollment, result);
        }

        [Fact]
        public async Task CreateEnrollmentAsync_Null_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateEnrollmentAsync(null!));
        }

        [Fact]
        public async Task CreateEnrollmentAsync_AlreadyEnrolled_ThrowsInvalidOperationException()
        {
            var existing = CreateValidEnrollment();
            var newEnrollment = CreateValidEnrollment();

            _repositoryMock.Setup(r => r.GetAllEnrollmentsAsync())
                .ReturnsAsync(new List<StudentOlympiadEnrollment> { existing });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateEnrollmentAsync(newEnrollment));
        }

        [Theory]
        [InlineData(999, 1, 1)]
        [InlineData(1, 999, 1)]
        [InlineData(1, 1, 999)]
        public async Task CreateEnrollmentAsync_NoMatch_DoesNotThrow(int userId, int olympiadId, int academicYearId)
        {
            var existing = CreateValidEnrollment();
            var newEnrollment = new StudentOlympiadEnrollment
            {
                UserId = userId,
                OlympiadId = olympiadId,
                AcademicYearId = academicYearId,
                EnrollmentStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.GetAllEnrollmentsAsync())
                .ReturnsAsync(new List<StudentOlympiadEnrollment> { existing });

            _repositoryMock.Setup(r => r.CreateEnrollmentAsync(It.IsAny<StudentOlympiadEnrollment>()))
                .ReturnsAsync(newEnrollment);

            var result = await _service.CreateEnrollmentAsync(newEnrollment);

            Assert.Equal(newEnrollment, result);
        }

        [Fact]
        public async Task CreateEnrollmentAsync_SendsEmail_WhenUserAndOlympiadExist()
        {
            var enrollment = CreateValidEnrollment();

            var user = new User
            {
                UserId = enrollment.UserId,
                Email = "student@example.com",
                Name = "Test Student",
                DateOfBirth = new DateTime(2005, 1, 1),
                AcademicYearId = 1,
                Username = "teststudent",
                Password = "TestPassword123"
            };

            var olympiad = new Olympiad
            {
                OlympiadId = enrollment.OlympiadId,
                Subject = "Математика",
                Location = "София",
                DateOfOlympiad = new DateTime(2025, 5, 10),
                StartTime = new DateTime(2025, 5, 10, 10, 0, 0),
                Round = "Областен",
                ClassNumber = 12
            };

            _repositoryMock.Setup(r => r.GetAllEnrollmentsAsync())
                .ReturnsAsync(new List<StudentOlympiadEnrollment>());
            _repositoryMock.Setup(r => r.CreateEnrollmentAsync(enrollment))
                .ReturnsAsync(enrollment);

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(enrollment.UserId))
                .ReturnsAsync(user);
            _olympiadRepositoryMock.Setup(r => r.GetOlympiadByIdAsync(enrollment.OlympiadId))
                .ReturnsAsync(olympiad);

            _emailServiceMock.Setup(e =>
                e.SendEmailAsync(
                    user.Email,
                    It.Is<string>(s => s.Contains("олимпиадата по")),
                    It.Is<string>(b => b.Contains("Уважаеми ученик") && b.Contains(olympiad.Location)),
                    null
                )
            ).Returns(Task.CompletedTask).Verifiable();

            var result = await _service.CreateEnrollmentAsync(enrollment);

            Assert.Equal(enrollment, result);
            _emailServiceMock.Verify();
        }

        [Fact]
        public async Task CreateEnrollmentAsync_SendsEmail_WhenOlympiadHasNoStartTime()
        {
            var enrollment = CreateValidEnrollment();

            var user = new User
            {
                UserId = enrollment.UserId,
                Email = "student@example.com",
                Name = "Test Student",
                DateOfBirth = new DateTime(2005, 1, 1),
                AcademicYearId = 1,
                Username = "teststudent",
                Password = "TestPassword123"
            };

            var olympiad = new Olympiad
            {
                OlympiadId = enrollment.OlympiadId,
                Subject = "Физика",
                Location = "Варна",
                DateOfOlympiad = new DateTime(2025, 6, 5),
                StartTime = null,
                Round = "Областен",
                ClassNumber = 11
            };

            _repositoryMock.Setup(r => r.GetAllEnrollmentsAsync())
                .ReturnsAsync(new List<StudentOlympiadEnrollment>());
            _repositoryMock.Setup(r => r.CreateEnrollmentAsync(enrollment))
                .ReturnsAsync(enrollment);

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(enrollment.UserId))
                .ReturnsAsync(user);
            _olympiadRepositoryMock.Setup(r => r.GetOlympiadByIdAsync(enrollment.OlympiadId))
                .ReturnsAsync(olympiad);

            _emailServiceMock.Setup(e =>
                e.SendEmailAsync(
                    user.Email,
                    It.Is<string>(s => s.Contains("олимпиадата по")),
                    It.Is<string>(b => b.Contains("няма конкретен час")),
                    null
                )
            ).Returns(Task.CompletedTask).Verifiable();

            var result = await _service.CreateEnrollmentAsync(enrollment);

            Assert.Equal(enrollment, result);
            _emailServiceMock.Verify();
        }


        [Fact]
        public async Task UpdateEnrollmentAsync_ValidCall_CallsRepository()
        {
            var updated = new StudentOlympiadEnrollment
            {
                EnrollmentId = 1,
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Approved",
                CreatedAt = DateTime.UtcNow
            };
            _repositoryMock.Setup(r => r.UpdateEnrollmentAsync(1, updated)).ReturnsAsync(updated);

            var result = await _service.UpdateEnrollmentAsync(1, updated);

            Assert.NotNull(result);
            Assert.Equal("Approved", result.EnrollmentStatus);
        }

        [Fact]
        public async Task UpdateEnrollmentAsync_NullUpdate_ThrowsException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.UpdateEnrollmentAsync(1, null!));
        }

        [Fact]
        public async Task DeleteEnrollmentAsync_DeletesEnrollment()
        {
            _repositoryMock.Setup(r => r.DeleteEnrollmentAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteEnrollmentAsync(1);

            Assert.True(result);
        }
    }
}
