using OlympiadApi.Models;
using OlympiadApi.Repositories;

namespace OlympiadApi.Services
{
    public class StudentOlympiadEnrollmentService
    {
        private readonly IStudentOlympiadEnrollmentRepository _repository;

        public StudentOlympiadEnrollmentService(IStudentOlympiadEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StudentOlympiadEnrollment>> GetAllEnrollmentsAsync()
        {
            return await _repository.GetAllEnrollmentsAsync();
        }

        public async Task<StudentOlympiadEnrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _repository.GetEnrollmentByIdAsync(id);
        }

       public async Task<StudentOlympiadEnrollment> CreateEnrollmentAsync(StudentOlympiadEnrollment enrollment)
        {
            if (enrollment == null)
                throw new ArgumentNullException(nameof(enrollment));

            var existingEnrollment = await _repository.GetAllEnrollmentsAsync();
            var alreadyEnrolled = existingEnrollment
                .Any(e => e.UserId == enrollment.UserId && e.OlympiadId == enrollment.OlympiadId && e.AcademicYearId == enrollment.AcademicYearId);

            if (alreadyEnrolled)
            {
                throw new InvalidOperationException("Student is already enrolled in this Olympiad for the given academic year.");
            }

            return await _repository.CreateEnrollmentAsync(enrollment);
        }

        public async Task<StudentOlympiadEnrollment?> UpdateEnrollmentAsync(int id, StudentOlympiadEnrollment updatedEnrollment)
        {
            if (updatedEnrollment == null)
                throw new ArgumentNullException(nameof(updatedEnrollment));

            return await _repository.UpdateEnrollmentAsync(id, updatedEnrollment);
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            return await _repository.DeleteEnrollmentAsync(id);
        }
    }
}
