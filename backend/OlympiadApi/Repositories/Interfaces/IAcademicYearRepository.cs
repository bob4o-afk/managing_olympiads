using OlympiadApi.Models;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IAcademicYearRepository
    {
        Task AddAcademicYearAsync(AcademicYear academicYear);
        Task<IEnumerable<AcademicYear>> GetAllAcademicYearsAsync();
        Task<AcademicYear?> GetAcademicYearByIdAsync(int id);
    }
}
