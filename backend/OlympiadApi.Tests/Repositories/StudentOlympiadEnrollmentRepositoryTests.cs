using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Implementations;

namespace OlympiadApi.Tests.Repositories
{
    public class StudentOlympiadEnrollmentRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        private void SeedDependencies(ApplicationDbContext context)
        {
            context.AcademicYear.Add(new AcademicYear
            {
                AcademicYearId = 1,
                StartYear = 2023,
                EndYear = 2024
            });

            context.SaveChanges();

            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                Name = "Test User",
                DateOfBirth = new DateTime(2005, 1, 1),
                Password = "Password123!",
                AcademicYearId = 1
            });

            context.Olympiads.Add(new Olympiad
            {
                OlympiadId = 1,
                Subject = "Math",
                Round = "First",
                Location = "Sofia",
                ClassNumber = 12,
                DateOfOlympiad = DateTime.UtcNow.AddDays(10),
                AcademicYearId = 1
            });

            context.SaveChanges();
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StudentOlympiadEnrollmentRepository(null!));
            Assert.Equal("context", ex.ParamName);
        }

        [Fact]
        public async Task CreateEnrollmentAsync_SavesEnrollment()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            var repo = new StudentOlympiadEnrollmentRepository(context);

            var enrollment = new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                User = context.Users.FirstOrDefault(u => u.UserId == 1),
                Olympiad = context.Olympiads.FirstOrDefault(o => o.OlympiadId == 1),
                AcademicYear = context.AcademicYear.FirstOrDefault(a => a.AcademicYearId == 1),
            };

            var result = await repo.CreateEnrollmentAsync(enrollment);
            Assert.NotNull(result);
            Assert.True(result.EnrollmentId > 0);
            Assert.NotEqual(default, result.CreatedAt);
        }

        [Fact]
        public async Task GetAllEnrollmentsAsync_ReturnsAll()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            context.StudentOlympiadEnrollment.Add(new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var repo = new StudentOlympiadEnrollmentRepository(context);
            var result = await repo.GetAllEnrollmentsAsync();
            Assert.Single(result);
        }

        [Fact]
        public async Task GetEnrollmentByIdAsync_ReturnsCorrectEnrollment()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            var enrollment = new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            context.StudentOlympiadEnrollment.Add(enrollment);
            await context.SaveChangesAsync();

            var repo = new StudentOlympiadEnrollmentRepository(context);
            var result = await repo.GetEnrollmentByIdAsync(enrollment.EnrollmentId);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEnrollmentsByUserIdAsync_ReturnsEnrollments()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            context.StudentOlympiadEnrollment.Add(new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Confirmed",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var repo = new StudentOlympiadEnrollmentRepository(context);
            var result = await repo.GetEnrollmentsByUserIdAsync(1);
            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateEnrollmentAsync_UpdatesData()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            var enrollment = new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            context.StudentOlympiadEnrollment.Add(enrollment);
            await context.SaveChangesAsync();

            var repo = new StudentOlympiadEnrollmentRepository(context);
            var update = new StudentOlympiadEnrollment
            {
                UserId = enrollment.UserId,
                OlympiadId = enrollment.OlympiadId,
                AcademicYearId = enrollment.AcademicYearId,
                CreatedAt = enrollment.CreatedAt,
                EnrollmentStatus = "Confirmed",
                Score = 99,
                StatusHistory = new Dictionary<string, object> { ["status"] = "Updated" }
            };

            var updated = await repo.UpdateEnrollmentAsync(enrollment.EnrollmentId, update);
            Assert.NotNull(updated);
            Assert.Equal("Confirmed", updated.EnrollmentStatus);
            Assert.Equal(99, updated.Score);
            Assert.NotEqual(default, updated.UpdatedAt);
            Assert.True(updated.StatusHistory?.ContainsKey("status"));
        }

        [Fact]
        public async Task UpdateEnrollmentAsync_ReturnsNull_WhenNotFound()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            var repo = new StudentOlympiadEnrollmentRepository(context);

            var update = new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                CreatedAt = DateTime.UtcNow,
                EnrollmentStatus = "Rejected",
                Score = 50,
                StatusHistory = new Dictionary<string, object>()
            };

            var result = await repo.UpdateEnrollmentAsync(1234, update);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteEnrollmentAsync_Deletes_WhenFound()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            var enrollment = new StudentOlympiadEnrollment
            {
                UserId = 1,
                OlympiadId = 1,
                AcademicYearId = 1,
                EnrollmentStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            context.StudentOlympiadEnrollment.Add(enrollment);
            await context.SaveChangesAsync();

            var repo = new StudentOlympiadEnrollmentRepository(context);
            var result = await repo.DeleteEnrollmentAsync(enrollment.EnrollmentId);
            Assert.True(result);
            Assert.Empty(context.StudentOlympiadEnrollment);
        }

        [Fact]
        public async Task DeleteEnrollmentAsync_ReturnsFalse_WhenNotFound()
        {
            using var context = GetInMemoryDbContext();
            SeedDependencies(context);

            var repo = new StudentOlympiadEnrollmentRepository(context);
            var result = await repo.DeleteEnrollmentAsync(999);
            Assert.False(result);
        }
    }
}
