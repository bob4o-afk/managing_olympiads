using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class AcademicYearService
    {
        private readonly ApplicationDbContext _context;

        public AcademicYearService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddAcademicYear(int startYear, int endYear)
        {
            var academicYear = new AcademicYear { StartYear = startYear, EndYear = endYear };
            _context.AcademicYear.Add(academicYear);
            _context.SaveChanges();
        }

        public IEnumerable<AcademicYear> GetAllAcademicYears()
        {
            return _context.AcademicYear.ToList();
        }
    }
}
