using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Repositories.Implementations
{
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly ApplicationDbContext _context;

        public AcademicYearRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddAcademicYear(AcademicYear academicYear)
        {
            _context.AcademicYear.Add(academicYear);
            _context.SaveChanges();
        }

        public IEnumerable<AcademicYear> GetAllAcademicYears()
        {
            return _context.AcademicYear.ToList();
        }

        public AcademicYear? GetAcademicYearById(int id)
        {
            return _context.AcademicYear.FirstOrDefault(ay => ay.AcademicYearId == id);
        }
    }
}
