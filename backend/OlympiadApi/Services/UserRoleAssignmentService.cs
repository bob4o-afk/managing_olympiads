using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class UserRoleAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public UserRoleAssignmentService(ApplicationDbContext context)
        {
            _context = context;
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
            assignment.AssignedAt = DateTime.UtcNow;
            _context.UserRoleAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var assignment = await _context.UserRoleAssignments.FindAsync(id);
            if (assignment == null) return false;

            _context.UserRoleAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
