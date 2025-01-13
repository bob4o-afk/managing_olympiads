using Microsoft.EntityFrameworkCore;
using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Services
{
    public class StudentOlympiadEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public StudentOlympiadEnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentOlympiadEnrollment>> GetAllEnrollmentsAsync()
        {
            return await _context.StudentOlympiadEnrollment
                .Include(e => e.User)
                .Include(e => e.Olympiad)
                .Include(e => e.AcademicYear)
                .ToListAsync();
        }

        public async Task<StudentOlympiadEnrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _context.StudentOlympiadEnrollment
                .Include(e => e.User)
                .Include(e => e.Olympiad)
                .Include(e => e.AcademicYear)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);
        }

        public async Task<StudentOlympiadEnrollment> CreateEnrollmentAsync(StudentOlympiadEnrollment enrollment)
        {
            enrollment.CreatedAt = DateTime.UtcNow;
            _context.StudentOlympiadEnrollment.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<StudentOlympiadEnrollment?> UpdateEnrollmentAsync(int id, StudentOlympiadEnrollment updatedEnrollment)
        {
            var enrollment = await _context.StudentOlympiadEnrollment.FindAsync(id);
            if (enrollment == null) return null;

            enrollment.EnrollmentStatus = updatedEnrollment.EnrollmentStatus;
            enrollment.StatusHistory = updatedEnrollment.StatusHistory;
            enrollment.Score = updatedEnrollment.Score;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _context.StudentOlympiadEnrollment.FindAsync(id);
            if (enrollment == null) return false;

            _context.StudentOlympiadEnrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
