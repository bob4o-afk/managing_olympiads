using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class OlympiadService
    {
        private readonly ApplicationDbContext _context;

        public OlympiadService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddOlympiad(string subject, DateTime dateofolympiad, string round, string location, int classNumber, int academicYearId)
        {
            var olympiad = new Olympiad
            {
                Subject = subject,
                DateOfOlympiad = dateofolympiad,
                Round = round,
                Location = location,
                ClassNumber = classNumber,
                AcademicYearId = academicYearId
            };

            _context.Olympiads.Add(olympiad);
            _context.SaveChanges();
        }

        public IEnumerable<Olympiad> GetAllOlympiads()
        {
            return _context.Olympiads.ToList();
        }
    }
}
