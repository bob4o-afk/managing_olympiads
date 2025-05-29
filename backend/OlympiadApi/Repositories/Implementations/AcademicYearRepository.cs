using Microsoft.EntityFrameworkCore;
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

        public async Task AddAcademicYearAsync(AcademicYear academicYear)
        {
            await _context.AcademicYear.AddAsync(academicYear);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AcademicYear>> GetAllAcademicYearsAsync()
        {
            return await _context.AcademicYear.ToListAsync();
        }

        public async Task<AcademicYear?> GetAcademicYearByIdAsync(int id)
        {
            return await _context.AcademicYear.FirstOrDefaultAsync(ay => ay.AcademicYearId == id);
        }
    }
}
