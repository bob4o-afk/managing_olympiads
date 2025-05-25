using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IUserRoleAssignmentService
    {
        Task<List<UserRoleAssignment>> GetAllAssignmentsAsync();
        Task<UserRoleAssignment?> GetAssignmentByIdAsync(int id);
        Task<UserRoleAssignment> CreateAssignmentAsync(UserRoleAssignment assignment);
        Task<bool> DeleteAssignmentAsync(int id);
    }
}
