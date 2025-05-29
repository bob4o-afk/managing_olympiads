using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        // Get all roles
        public async Task<List<Role>> GetRolesAsync()
        {
            return await _roleRepository.GetRolesAsync();
        }

        // Create a new role
        public async Task<Role?> CreateRoleAsync(Role role)
        {
            if (role == null || string.IsNullOrEmpty(role.RoleName))
            {
                return null;
            }

            return await _roleRepository.CreateRoleAsync(role);
        }

        // Delete a role by id
        public async Task<bool> DeleteRoleAsync(int id)
        {
            return await _roleRepository.DeleteRoleAsync(id);
        }
    }
}
