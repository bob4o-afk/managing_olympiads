using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IOlympiadService
    {
        IEnumerable<Olympiad> GetAllOlympiads();
        Olympiad? GetOlympiadById(int id);
        void AddOlympiad(Olympiad olympiad);
    }
}
