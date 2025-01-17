using OlympiadApi.Models;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IAcademicYearRepository
    {
        void AddAcademicYear(AcademicYear academicYear);
        IEnumerable<AcademicYear> GetAllAcademicYears();
        AcademicYear? GetAcademicYearById(int id);
    }
}
