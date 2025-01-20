using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class AcademicYearService
    {
        private readonly IAcademicYearRepository _academicYearRepository;

        public AcademicYearService(IAcademicYearRepository academicYearRepository)
        {
            _academicYearRepository = academicYearRepository;
        }

        public void AddAcademicYear(int startYear, int endYear)
        {
            var academicYear = new AcademicYear { StartYear = startYear, EndYear = endYear };
            _academicYearRepository.AddAcademicYear(academicYear);
        }

        public IEnumerable<AcademicYear> GetAllAcademicYears()
        {
            return _academicYearRepository.GetAllAcademicYears();
        }

        public AcademicYear? GetAcademicYearById(int id)
        {
            return _academicYearRepository.GetAcademicYearById(id);
        }
    }
}
