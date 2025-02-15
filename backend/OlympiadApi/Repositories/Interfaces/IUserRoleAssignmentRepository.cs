using OlympiadApi.Models;

namespace OlympiadApi.Repositories.Interfaces
{
    public interface IUserRoleAssignmentRepository
    {
        Task<List<UserRoleAssignment>> GetAllAssignmentsAsync();
        Task<UserRoleAssignment?> GetAssignmentByIdAsync(int id);
        Task<UserRoleAssignment> CreateAssignmentAsync(UserRoleAssignment assignment);
        Task<bool> DeleteAssignmentAsync(int id);
    }
}
