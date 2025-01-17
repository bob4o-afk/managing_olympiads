using OlympiadApi.Data;
using OlympiadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace OlympiadApi.Repositories
{
    public class OlympiadRepository : IOlympiadRepository
    {
        private readonly ApplicationDbContext _context;

        public OlympiadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddOlympiad(Olympiad olympiad)
        {
            if (olympiad == null) 
                throw new ArgumentNullException(nameof(olympiad));

            if (string.IsNullOrEmpty(olympiad.Subject))
                throw new ArgumentException("Olympiad subject cannot be null or empty.", nameof(olympiad.Subject));

            if (olympiad.DateOfOlympiad == default)
                throw new ArgumentException("Olympiad DateOfOlympiad cannot be default.", nameof(olympiad.DateOfOlympiad));

            _context.Olympiads.Add(olympiad);
            _context.SaveChanges();
        }


        public IEnumerable<Olympiad> GetAllOlympiads()
        {
            return _context.Olympiads?.Include(o => o.AcademicYear).ToList() ?? new List<Olympiad>();
        }

        public Olympiad GetOlympiadById(int id)
        {
            var olympiad = _context.Olympiads?.Include(o => o.AcademicYear)
                                             .FirstOrDefault(o => o.OlympiadId == id);
            
            if (olympiad == null)
                throw new KeyNotFoundException($"Olympiad with ID {id} not found.");

            return olympiad;
        }
    }
}
