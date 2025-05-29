using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Implementations;

namespace OlympiadApi.Tests.Repositories
{
    public class AcademicYearRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAcademicYear_AddsCorrectly()
        {
            using var context = GetInMemoryDbContext();
            var repository = new AcademicYearRepository(context);

            var year = new AcademicYear { StartYear = 2024, EndYear = 2025 };

            await repository.AddAcademicYearAsync(year);

            var result = context.AcademicYear.FirstOrDefault();
            Assert.NotNull(result);
            Assert.Equal(2024, result!.StartYear);
            Assert.Equal(2025, result.EndYear);
        }

        [Fact]
        public async Task GetAllAcademicYears_ReturnsAll()
        {
            using var context = GetInMemoryDbContext();
            context.AcademicYear.AddRange(
                new AcademicYear { StartYear = 2022, EndYear = 2023 },
                new AcademicYear { StartYear = 2023, EndYear = 2024 });
            context.SaveChanges();

            var repository = new AcademicYearRepository(context);

            var result = (await repository.GetAllAcademicYearsAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, ay => ay.StartYear == 2022 && ay.EndYear == 2023);
            Assert.Contains(result, ay => ay.StartYear == 2023 && ay.EndYear == 2024);
        }

        [Fact]
        public async Task GetAcademicYearById_ReturnsCorrectYear()
        {
            using var context = GetInMemoryDbContext();
            context.AcademicYear.Add(new AcademicYear { AcademicYearId = 5, StartYear = 2021, EndYear = 2022 });
            context.SaveChanges();

            var repository = new AcademicYearRepository(context);

            var result = await repository.GetAcademicYearByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(2021, result!.StartYear);
            Assert.Equal(2022, result.EndYear);
        }

        [Fact]
        public async Task GetAcademicYearById_ReturnsNull_WhenNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repository = new AcademicYearRepository(context);

            var result = await repository.GetAcademicYearByIdAsync(999);

            Assert.Null(result);
        }
    }
}
