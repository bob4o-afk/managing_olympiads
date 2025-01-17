using OlympiadApi.Models;
using OlympiadApi.Data;
using Microsoft.EntityFrameworkCore;

namespace OlympiadApi.Services
{
    public class RoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all roles
        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        // Create a new role
        public async Task<Role?> CreateRoleAsync(Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return null;
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }
    }
}
