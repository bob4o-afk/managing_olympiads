using OlympiadApi.Models;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IOlympiadRepository
    {
        Task<Olympiad> AddOlympiadAsync(Olympiad olympiad);

        Task<IEnumerable<Olympiad>> GetAllOlympiadsAsync();

        Task<Olympiad> GetOlympiadByIdAsync(int id);
    }
}
