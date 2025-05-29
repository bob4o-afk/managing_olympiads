using Moq;
using OlympiadApi.Services;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Models;

namespace OlympiadApi.Tests.Services
{
    public class AcademicYearServiceTests
    {
        private readonly AcademicYearService _academicYearService;
        private readonly Mock<IAcademicYearRepository> _academicYearRepositoryMock;

        public AcademicYearServiceTests()
        {
            _academicYearRepositoryMock = new Mock<IAcademicYearRepository>();
            _academicYearService = new AcademicYearService(_academicYearRepositoryMock.Object);
        }

        [Fact]
        public async Task AddAcademicYear_CreatesNewAcademicYear()
        {
            var startYear = 2023;
            var endYear = 2024;

            await _academicYearService.AddAcademicYearAsync(startYear, endYear);

            _academicYearRepositoryMock.Verify(r => r.AddAcademicYearAsync(It.Is<AcademicYear>(a =>
                a.StartYear == startYear && a.EndYear == endYear
            )), Times.Once);
        }

        [Fact]
        public async Task GetAllAcademicYears_ReturnsAcademicYears()
        {
            var academicYears = new List<AcademicYear>
            {
                new AcademicYear { AcademicYearId = 1, StartYear = 2022, EndYear = 2023 },
                new AcademicYear { AcademicYearId = 2, StartYear = 2023, EndYear = 2024 }
            };
            _academicYearRepositoryMock.Setup(repo => repo.GetAllAcademicYearsAsync()).ReturnsAsync(academicYears);

            var result = await _academicYearService.GetAllAcademicYearsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAcademicYearById_ValidId_ReturnsAcademicYear()
        {
            var academicYear = new AcademicYear { AcademicYearId = 1, StartYear = 2022, EndYear = 2023 };
            _academicYearRepositoryMock.Setup(r => r.GetAcademicYearByIdAsync(1)).ReturnsAsync(academicYear);

            var result = await _academicYearService.GetAcademicYearByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.AcademicYearId);
        }
    }
}
