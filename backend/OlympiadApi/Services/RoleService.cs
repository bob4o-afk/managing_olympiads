using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Services
{
    public class RoleService
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

            // to do check this - maybe add default permissions but they are added from the db si maybe it is not needed 
            // if (role.Permissions == null)
            // {
            //     role.Permissions = new Dictionary<string, object>
            //     {
            //         { PermissionType.Read.ToString(), true },
            //         { PermissionType.Write.ToString(), false },
            //     };
            // }

            return await _roleRepository.CreateRoleAsync(role);
        }

        // Delete a role by id
        public async Task<bool> DeleteRoleAsync(int id)
        {
            return await _roleRepository.DeleteRoleAsync(id);
        }
    }
}
