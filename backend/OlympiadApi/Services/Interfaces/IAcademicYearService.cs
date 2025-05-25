using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IAcademicYearService
    {
        IEnumerable<AcademicYear> GetAllAcademicYears();
        AcademicYear? GetAcademicYearById(int id);
        void AddAcademicYear(int startYear, int endYear);
    }
}
