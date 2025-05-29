using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IOlympiadService
    {
        Task<IEnumerable<Olympiad>> GetAllOlympiadsAsync();
        Task<Olympiad?> GetOlympiadByIdAsync(int id);
        Task<Olympiad> AddOlympiadAsync(Olympiad olympiad);
    }
}
