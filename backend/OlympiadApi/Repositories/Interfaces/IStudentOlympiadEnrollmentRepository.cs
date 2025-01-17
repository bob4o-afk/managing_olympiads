using OlympiadApi.Models;

namespace OlympiadApi.Repositories
{
    public interface IStudentOlympiadEnrollmentRepository
    {
        Task<List<StudentOlympiadEnrollment>> GetAllEnrollmentsAsync();
        Task<StudentOlympiadEnrollment?> GetEnrollmentByIdAsync(int id);
        Task<StudentOlympiadEnrollment> CreateEnrollmentAsync(StudentOlympiadEnrollment enrollment);
        Task<StudentOlympiadEnrollment?> UpdateEnrollmentAsync(int id, StudentOlympiadEnrollment updatedEnrollment);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}
