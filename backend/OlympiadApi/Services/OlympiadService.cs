using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class OlympiadService : IOlympiadService
    {
        private readonly IOlympiadRepository _repository;

        public OlympiadService(IOlympiadRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Olympiad> AddOlympiadAsync(Olympiad olympiad)
        {
            if (olympiad == null)
                throw new ArgumentNullException(nameof(olympiad));

            if (string.IsNullOrEmpty(olympiad.Subject))
                throw new ArgumentException("Subject is required.", nameof(olympiad.Subject));

            if (olympiad.DateOfOlympiad == default)
                throw new ArgumentException("Date of Olympiad is required.", nameof(olympiad.DateOfOlympiad));

            return await _repository.AddOlympiadAsync(olympiad);
        }

        public async Task<IEnumerable<Olympiad>> GetAllOlympiadsAsync()
        {
            return await _repository.GetAllOlympiadsAsync();
        }

        public async Task<Olympiad?> GetOlympiadByIdAsync(int id)
        {
            return await _repository.GetOlympiadByIdAsync(id);
        }
    }
}
