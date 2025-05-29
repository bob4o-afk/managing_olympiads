using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class AcademicYearService : IAcademicYearService
    {
        private readonly IAcademicYearRepository _academicYearRepository;

        public AcademicYearService(IAcademicYearRepository academicYearRepository)
        {
            _academicYearRepository = academicYearRepository;
        }

        public async Task AddAcademicYearAsync(int startYear, int endYear)
        {
            var academicYear = new AcademicYear { StartYear = startYear, EndYear = endYear };
            await _academicYearRepository.AddAcademicYearAsync(academicYear);
        }

        public async Task<IEnumerable<AcademicYear>> GetAllAcademicYearsAsync()
        {
            return await _academicYearRepository.GetAllAcademicYearsAsync();
        }

        public async Task<AcademicYear?> GetAcademicYearByIdAsync(int id)
        {
            return await _academicYearRepository.GetAcademicYearByIdAsync(id);
        }
    }
}
