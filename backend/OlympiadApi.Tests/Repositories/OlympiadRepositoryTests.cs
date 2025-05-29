using Microsoft.EntityFrameworkCore;
using Moq;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Implementations;

namespace OlympiadApi.Tests.Repositories
{
    public class OlympiadRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"OlympiadTestDb_{Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OlympiadRepository(null!));
            Assert.Equal("context", ex.ParamName);
        }

        [Fact]
        public async Task AddOlympiad_SuccessfullyAddsValidOlympiad()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var olympiad = new Olympiad
            {
                Subject = "Math",
                DateOfOlympiad = DateTime.UtcNow.AddDays(10),
                Round = "First",
                Location = "Sofia",
                ClassNumber = 12,
                AcademicYearId = 1
            };

            await repo.AddOlympiadAsync(olympiad);

            var saved = context.Olympiads.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Math", saved.Subject);
        }

        [Fact]
        public async Task AddOlympiad_ThrowsException_WhenSubjectIsNull()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var olympiad = new Olympiad
            {
                Subject = null!,
                DateOfOlympiad = DateTime.UtcNow,
                Round = "First",
                Location = "Sofia",
                ClassNumber = 11,
                AcademicYearId = 1
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repo.AddOlympiadAsync(olympiad));
            Assert.Contains("Olympiad subject cannot be null or empty", ex.Message);
        }

        [Fact]
        public async Task AddOlympiad_ThrowsException_WhenDateIsDefault()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var olympiad = new Olympiad
            {
                Subject = "Informatics",
                DateOfOlympiad = default,
                Round = "Second",
                Location = "Plovdiv",
                ClassNumber = 10,
                AcademicYearId = 1
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repo.AddOlympiadAsync(olympiad));
            Assert.Contains("Olympiad DateOfOlympiad cannot be default", ex.Message);
        }

        [Fact]
        public async Task GetAllOlympiads_ReturnsOlympiads()
        {
            using var context = GetInMemoryDbContext();

            var year = new AcademicYear { StartYear = 2023, EndYear = 2024 };
            context.AcademicYear.Add(year);
            context.SaveChanges();

            context.Olympiads.AddRange(
                new Olympiad
                {
                    Subject = "Math",
                    DateOfOlympiad = DateTime.UtcNow,
                    Round = "First",
                    Location = "Sofia",
                    ClassNumber = 11,
                    AcademicYearId = year.AcademicYearId
                },
                new Olympiad
                {
                    Subject = "Physics",
                    DateOfOlympiad = DateTime.UtcNow.AddDays(5),
                    Round = "Second",
                    Location = "Varna",
                    ClassNumber = 12,
                    AcademicYearId = year.AcademicYearId
                });
            context.SaveChanges();

            var repo = new OlympiadRepository(context);

            var result = (await repo.GetAllOlympiadsAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, o => Assert.NotNull(o.AcademicYear));
        }


        [Fact]
        public async Task GetAllOlympiads_ReturnsEmptyList_WhenOlympiadsIsNull()
        {
            var mockContext = new Mock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options
            );

            mockContext.Setup(c => c.Olympiads).Returns((DbSet<Olympiad>)null!);

            var repo = new OlympiadRepository(mockContext.Object);

            var result = await repo.GetAllOlympiadsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOlympiadById_ReturnsCorrectOlympiad()
        {
            using var context = GetInMemoryDbContext();

            var year = new AcademicYear { StartYear = 2022, EndYear = 2023 };
            context.AcademicYear.Add(year);
            context.SaveChanges();

            var olympiad = new Olympiad
            {
                OlympiadId = 99,
                Subject = "Biology",
                DateOfOlympiad = DateTime.UtcNow,
                Round = "First",
                Location = "Burgas",
                ClassNumber = 9,
                AcademicYearId = year.AcademicYearId
            };

            context.Olympiads.Add(olympiad);
            context.SaveChanges();

            var repo = new OlympiadRepository(context);
            var result = await repo.GetOlympiadByIdAsync(99);

            Assert.NotNull(result);
            Assert.Equal("Biology", result.Subject);
            Assert.NotNull(result.AcademicYear);
            Assert.Equal(2022, result.AcademicYear.StartYear);
        }

        [Fact]
        public async Task GetOlympiadById_Throws_WhenNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.GetOlympiadByIdAsync(1000));
            Assert.Contains("Olympiad with ID 1000 not found", ex.Message);
        }

        [Fact]
        public async Task GetOlympiadById_ThrowsKeyNotFound_WhenDbSetIsNull()
        {
            var mockContext = new Mock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options
            );

            mockContext.Setup(c => c.Olympiads).Returns((DbSet<Olympiad>)null!);

            var repo = new OlympiadRepository(mockContext.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.GetOlympiadByIdAsync(1));
            Assert.Equal("Olympiads DbSet is not initialized.", ex.Message);
        }

        [Fact]
        public async Task AddOlympiad_ThrowsArgumentNullException_WhenOlympiadIsNull()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddOlympiadAsync(null!));
            Assert.Equal("olympiad", ex.ParamName);
        }

        [Fact]
        public async Task AddOlympiad_ThrowsArgumentException_WhenSubjectIsEmpty()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var olympiad = new Olympiad
            {
                Subject = "",
                DateOfOlympiad = DateTime.UtcNow,
                Round = "First",
                Location = "Test",
                ClassNumber = 9,
                AcademicYearId = 1
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repo.AddOlympiadAsync(olympiad));
            Assert.Equal("Olympiad subject cannot be null or empty. (Parameter 'Subject')", ex.Message);
        }

        [Fact]
        public async Task AddOlympiad_ThrowsArgumentException_WhenDateOfOlympiadIsDefault()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var olympiad = new Olympiad
            {
                Subject = "Math",
                DateOfOlympiad = default,
                Round = "Test",
                Location = "Test",
                ClassNumber = 9,
                AcademicYearId = 1
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repo.AddOlympiadAsync(olympiad));
            Assert.Contains("Olympiad DateOfOlympiad cannot be default", ex.Message);
        }

        [Fact]
        public async Task AddOlympiad_ThrowsArgumentException_WhenSubjectIsNull()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var olympiad = new Olympiad
            {
                Subject = null!,
                DateOfOlympiad = DateTime.UtcNow,
                Round = "First",
                Location = "Room A",
                ClassNumber = 9,
                AcademicYearId = 1
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => repo.AddOlympiadAsync(olympiad));
            Assert.Contains("Olympiad subject cannot be null or empty.", ex.Message);
        }

        [Fact]
        public async Task GetOlympiadById_ThrowsKeyNotFound_WhenOlympiadNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OlympiadRepository(context);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.GetOlympiadByIdAsync(999));
            Assert.Equal("Olympiad with ID 999 not found.", ex.Message);
        }
    }
}
