using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IAcademicYearService
    {
        Task<IEnumerable<AcademicYear>> GetAllAcademicYearsAsync();
        Task <AcademicYear?> GetAcademicYearByIdAsync(int id);
        Task AddAcademicYearAsync(int startYear, int endYear);
    }
}
