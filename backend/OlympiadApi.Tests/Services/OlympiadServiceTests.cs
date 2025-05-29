using Moq;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services;

namespace OlympiadApi.Tests.Services
{
    public class OlympiadServiceTests
    {
        private readonly Mock<IOlympiadRepository> _repositoryMock;
        private readonly OlympiadService _service;

        public OlympiadServiceTests()
        {
            _repositoryMock = new Mock<IOlympiadRepository>();
            _service = new OlympiadService(_repositoryMock.Object);
        }

        private Olympiad CreateValidOlympiad() => new Olympiad
        {
            Subject = "Math",
            DateOfOlympiad = DateTime.Today,
            Round = "First",
            Location = "TUES",
            ClassNumber = 10,
            AcademicYearId = 1
        };

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OlympiadService(null!));
            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public async Task AddOlympiad_ValidOlympiad_CallsRepository()
        {
            var olympiad = CreateValidOlympiad();

            await _service.AddOlympiadAsync(olympiad);

            _repositoryMock.Verify(r => r.AddOlympiadAsync(olympiad), Times.Once);
        }

        [Fact]
        public async Task AddOlympiad_NullOlympiad_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddOlympiadAsync(null!));
        }

        [Fact]
        public async Task AddOlympiad_EmptySubject_ThrowsArgumentException()
        {
            var olympiad = CreateValidOlympiad();
            olympiad.Subject = "";

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddOlympiadAsync(olympiad));
            Assert.Equal("Subject is required. (Parameter 'Subject')", ex.Message);
        }

        [Fact]
        public async Task AddOlympiad_DefaultDate_ThrowsArgumentException()
        {
            var olympiad = CreateValidOlympiad();
            olympiad.DateOfOlympiad = default;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddOlympiadAsync(olympiad));
            Assert.Equal("Date of Olympiad is required. (Parameter 'DateOfOlympiad')", ex.Message);
        }

        [Fact]
        public async Task GetAllOlympiads_ReturnsOlympiads()
        {
            var olympiads = new List<Olympiad>
            {
                CreateValidOlympiad(),
                CreateValidOlympiad()
            };

            _repositoryMock.Setup(r => r.GetAllOlympiadsAsync()).ReturnsAsync(olympiads);

            var result = (await _service.GetAllOlympiadsAsync()).ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetOlympiadById_ValidId_ReturnsOlympiad()
        {
            var olympiad = CreateValidOlympiad();
            olympiad.Subject = "Chemistry";

            _repositoryMock.Setup(r => r.GetOlympiadByIdAsync(1)).ReturnsAsync(olympiad);

            var result = await _service.GetOlympiadByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Chemistry", result.Subject);
        }
    }
}
