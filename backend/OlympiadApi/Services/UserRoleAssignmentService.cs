using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class UserRoleAssignmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRoleAssignmentService> _logger;

        public UserRoleAssignmentService(ApplicationDbContext context, ILogger<UserRoleAssignmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserRoleAssignment>> GetAllAssignmentsAsync()
        {
            try
            {
                return await _context.UserRoleAssignments
                    .Include(ura => ura.User)
                    .Include(ura => ura.Role)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user role assignments.");
                throw new Exception("An error occurred while retrieving user role assignments.");
            }
        }

        public async Task<UserRoleAssignment?> GetAssignmentByIdAsync(int id)
        {
            try
            {
                return await _context.UserRoleAssignments
                    .Include(ura => ura.User)
                    .Include(ura => ura.Role)
                    .FirstOrDefaultAsync(ura => ura.AssignmentId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user role assignment with ID: {id}.");
                throw new Exception($"An error occurred while retrieving the user role assignment with ID: {id}.");
            }
        }

        public async Task<UserRoleAssignment> CreateAssignmentAsync(UserRoleAssignment assignment)
        {
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.UserId == assignment.UserId);
                if (!userExists)
                {
                    throw new ArgumentException($"User with ID {assignment.UserId} does not exist.");
                }

                var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == assignment.RoleId);
                if (!roleExists)
                {
                    throw new ArgumentException($"Role with ID {assignment.RoleId} does not exist.");
                }


                assignment.AssignedAt = DateTime.UtcNow;

                _context.UserRoleAssignments.Add(assignment);

                await _context.SaveChangesAsync();

                return assignment;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation failed when creating user role assignment.");
                throw new Exception(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error occurred while creating user role assignment.");
                throw new Exception("A database error occurred while creating the user role assignment.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating user role assignment.");
                throw new Exception("An unexpected error occurred while creating the user role assignment.");
            }
        }


        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            try
            {
                var assignment = await _context.UserRoleAssignments.FindAsync(id);
                if (assignment == null)
                {
                    throw new ArgumentException($"Assignment with ID {id} not found.");
                }

                _context.UserRoleAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation failed when deleting user role assignment.");
                throw new Exception(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error occurred.");
                throw new Exception("A database error occurred while deleting the user role assignment.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user role assignment.");
                throw new Exception("An error occurred while deleting the user role assignment.");
            }
        }
    }
}
