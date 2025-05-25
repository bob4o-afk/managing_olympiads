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
        public void AddOlympiad_ValidOlympiad_CallsRepository()
        {
            var olympiad = CreateValidOlympiad();

            _service.AddOlympiad(olympiad);

            _repositoryMock.Verify(r => r.AddOlympiad(olympiad), Times.Once);
        }

        [Fact]
        public void AddOlympiad_NullOlympiad_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.AddOlympiad(null!));
        }

        [Fact]
        public void AddOlympiad_EmptySubject_ThrowsArgumentException()
        {
            var olympiad = CreateValidOlympiad();
            olympiad.Subject = "";

            var ex = Assert.Throws<ArgumentException>(() => _service.AddOlympiad(olympiad));
            Assert.Equal("Subject is required. (Parameter 'Subject')", ex.Message);
        }

        [Fact]
        public void AddOlympiad_DefaultDate_ThrowsArgumentException()
        {
            var olympiad = CreateValidOlympiad();
            olympiad.DateOfOlympiad = default;

            var ex = Assert.Throws<ArgumentException>(() => _service.AddOlympiad(olympiad));
            Assert.Equal("Date of Olympiad is required. (Parameter 'DateOfOlympiad')", ex.Message);
        }

        [Fact]
        public void GetAllOlympiads_ReturnsOlympiads()
        {
            var olympiads = new List<Olympiad>
            {
                CreateValidOlympiad(),
                CreateValidOlympiad()
            };

            _repositoryMock.Setup(r => r.GetAllOlympiads()).Returns(olympiads);

            var result = _service.GetAllOlympiads().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetOlympiadById_ValidId_ReturnsOlympiad()
        {
            var olympiad = CreateValidOlympiad();
            olympiad.Subject = "Chemistry";

            _repositoryMock.Setup(r => r.GetOlympiadById(1)).Returns(olympiad);

            var result = _service.GetOlympiadById(1);

            Assert.NotNull(result);
            Assert.Equal("Chemistry", result.Subject);
        }
    }
}
