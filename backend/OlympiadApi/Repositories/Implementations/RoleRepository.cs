using OlympiadApi.Data;
using OlympiadApi.Models;
using Microsoft.EntityFrameworkCore;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> CreateRoleAsync(Role role)
        {
            if (role == null || string.IsNullOrEmpty(role.RoleName))
            {
                return null;
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles
                                 .FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
