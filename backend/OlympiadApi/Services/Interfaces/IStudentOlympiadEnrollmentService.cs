using OlympiadApi.Models;

namespace OlympiadApi.Services.Interfaces
{
    public interface IStudentOlympiadEnrollmentService
    {
        Task<List<StudentOlympiadEnrollment>> GetAllEnrollmentsAsync();
        Task<StudentOlympiadEnrollment?> GetEnrollmentByIdAsync(int id);
        Task<List<StudentOlympiadEnrollment>> GetEnrollmentsByUserIdAsync(int userId);
        Task<StudentOlympiadEnrollment> CreateEnrollmentAsync(StudentOlympiadEnrollment enrollment);
        Task<StudentOlympiadEnrollment?> UpdateEnrollmentAsync(int id, StudentOlympiadEnrollment updatedEnrollment);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}
