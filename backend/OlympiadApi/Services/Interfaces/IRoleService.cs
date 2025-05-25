using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetRolesAsync();
        Task<Role?> CreateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id);
    }
}
