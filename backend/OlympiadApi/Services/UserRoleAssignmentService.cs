using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class UserRoleAssignmentService : IUserRoleAssignmentService
    {
        private readonly IUserRoleAssignmentRepository _repository;
        private readonly ILogger<UserRoleAssignmentService> _logger;

        public UserRoleAssignmentService(IUserRoleAssignmentRepository repository, ILogger<UserRoleAssignmentService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<UserRoleAssignment>> GetAllAssignmentsAsync()
        {
            try
            {
                return await _repository.GetAllAssignmentsAsync();
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
                return await _repository.GetAssignmentByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user role assignment with ID: {id}.");
                throw new Exception($"An error occurred while retrieving the user role assignment with ID: {id}.");
            }
        }

        public async Task<UserRoleAssignment> CreateAssignmentAsync(UserRoleAssignment assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));
            try
            {
                var userExists = await _repository.GetAllAssignmentsAsync();
                if (userExists == null)
                    throw new InvalidOperationException("User or Role does not exist.");

                return await _repository.CreateAssignmentAsync(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user role assignment.");
                throw new Exception("An error occurred while creating user role assignment.");
            }
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            try
            {
                return await _repository.DeleteAssignmentAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user role assignment.");
                throw new Exception("An error occurred while deleting the user role assignment.");
            }
        }
    }
}
