using OlympiadApi.Data;
using OlympiadApi.Models;
using Microsoft.EntityFrameworkCore;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Repositories.Implementations
{
    public class OlympiadRepository : IOlympiadRepository
    {
        private readonly ApplicationDbContext _context;

        public OlympiadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Olympiad> AddOlympiadAsync(Olympiad olympiad)
        {
            if (olympiad == null)
                throw new ArgumentNullException(nameof(olympiad));

            if (string.IsNullOrEmpty(olympiad.Subject))
                throw new ArgumentException("Olympiad subject cannot be null or empty.", nameof(olympiad.Subject));

            if (olympiad.DateOfOlympiad == default)
                throw new ArgumentException("Olympiad DateOfOlympiad cannot be default.", nameof(olympiad.DateOfOlympiad));

            await _context.Olympiads.AddAsync(olympiad);
            await _context.SaveChangesAsync();

            return olympiad;
        }

        public async Task<IEnumerable<Olympiad>> GetAllOlympiadsAsync()
        {
            if (_context.Olympiads == null)
                return new List<Olympiad>();

            return await _context.Olympiads.Include(o => o.AcademicYear).ToListAsync();
        }

        public async Task <Olympiad> GetOlympiadByIdAsync(int id)
        {
            if (_context.Olympiads == null)
                throw new InvalidOperationException("Olympiads DbSet is not initialized.");

            var olympiad = await _context.Olympiads.Include(o => o.AcademicYear).FirstOrDefaultAsync(o => o.OlympiadId == id);

            if (olympiad == null)
                throw new KeyNotFoundException($"Olympiad with ID {id} not found.");

            return olympiad;
        }
    }
}
