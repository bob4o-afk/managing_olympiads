using OlympiadApi.Models;

namespace OlympiadApi.Repositories
{
    public interface IOlympiadRepository
    {
        void AddOlympiad(Olympiad olympiad);

        IEnumerable<Olympiad> GetAllOlympiads();

        Olympiad GetOlympiadById(int id);
    }
}
