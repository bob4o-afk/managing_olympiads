using OlympiadApi.Models;
using OlympiadApi.Repositories;

namespace OlympiadApi.Services
{
    public class OlympiadService
    {
        private readonly IOlympiadRepository _repository;

        public OlympiadService(IOlympiadRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void AddOlympiad(Olympiad olympiad)
        {
            if (olympiad == null)
                throw new ArgumentNullException(nameof(olympiad));

            if (string.IsNullOrEmpty(olympiad.Subject))
                throw new ArgumentException("Subject is required.", nameof(olympiad.Subject));

            if (olympiad.DateOfOlympiad == default)
                throw new ArgumentException("Date of Olympiad is required.", nameof(olympiad.DateOfOlympiad));

            _repository.AddOlympiad(olympiad);
        }


        public IEnumerable<Olympiad> GetAllOlympiads()
        {
            return _repository.GetAllOlympiads();
        }

        public Olympiad GetOlympiadById(int id)
        {
            return _repository.GetOlympiadById(id);
        }
    }
}
