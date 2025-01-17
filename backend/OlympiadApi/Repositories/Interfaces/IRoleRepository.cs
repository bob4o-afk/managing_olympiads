using OlympiadApi.Models;

namespace OlympiadApi.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetRolesAsync();
        Task<Role?> CreateRoleAsync(Role role);
        Task<Role?> GetRoleByIdAsync(int id);
    }
}
