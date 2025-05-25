using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;

namespace OlympiadApi.Repositories.Implementations
{
    public class UserRoleAssignmentRepository : IUserRoleAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleAssignmentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<UserRoleAssignment>> GetAllAssignmentsAsync()
        {
            return await _context.UserRoleAssignments
                .Include(ura => ura.User)
                .Include(ura => ura.Role)
                .ToListAsync();
        }

        public async Task<UserRoleAssignment?> GetAssignmentByIdAsync(int id)
        {
            return await _context.UserRoleAssignments
                .Include(ura => ura.User)
                .Include(ura => ura.Role)
                .FirstOrDefaultAsync(ura => ura.AssignmentId == id);
        }

        public async Task<UserRoleAssignment> CreateAssignmentAsync(UserRoleAssignment assignment)
        {
            var user = await _context.Users.FindAsync(assignment.UserId);
            var role = await _context.Roles.FindAsync(assignment.RoleId);

            if (user == null || role == null)
                throw new InvalidOperationException("Invalid UserId or RoleId provided.");

            assignment.User = user;
            assignment.Role = role;

            _context.UserRoleAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return assignment;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var assignment = await _context.UserRoleAssignments.FindAsync(id);
            if (assignment == null)
                return false;

            _context.UserRoleAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
