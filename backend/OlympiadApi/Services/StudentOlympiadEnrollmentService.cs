using System.Globalization;
using OlympiadApi.Models;
using OlympiadApi.Repositories.Interfaces;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class StudentOlympiadEnrollmentService : IStudentOlympiadEnrollmentService
    {
        private readonly IStudentOlympiadEnrollmentRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IOlympiadRepository _olympiadRepository;
        private readonly IEmailService _emailService;

        public StudentOlympiadEnrollmentService(IStudentOlympiadEnrollmentRepository repository, IUserRepository userRepository, IOlympiadRepository olympiadRepository, IEmailService emailService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _olympiadRepository = olympiadRepository;
            _emailService = emailService;
        }

        public async Task<List<StudentOlympiadEnrollment>> GetAllEnrollmentsAsync()
        {
            return await _repository.GetAllEnrollmentsAsync();
        }

        public async Task<StudentOlympiadEnrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _repository.GetEnrollmentByIdAsync(id);
        }

        public async Task<List<StudentOlympiadEnrollment>> GetEnrollmentsByUserIdAsync(int userId)
        {
            return await _repository.GetEnrollmentsByUserIdAsync(userId);
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

            var created = await _repository.CreateEnrollmentAsync(enrollment);

            var user = await _userRepository.GetUserByIdAsync(enrollment.UserId);
            var olympiad = await _olympiadRepository.GetOlympiadByIdAsync(enrollment.OlympiadId);
            if (user != null && olympiad != null)
            {
                var date = olympiad.DateOfOlympiad.ToString("dddd, dd MMMM yyyy", new CultureInfo("bg-BG"));
                var time = olympiad.StartTime?.ToString("HH:mm") ?? "няма конкретен час";

                var subject = $"Записване за олимпиадата по {olympiad.Subject}";
                var body = $@"
                <p>Уважаеми ученик,</p>
                <p>Вие успешно се записахте за олимпиадата по <strong>{olympiad.Subject}</strong>.</p>
                <p>
                    Място: {olympiad.Location}<br>
                    Дата: {date}<br>
                    Начален час: {time}
                </p>
                <p>Поздрави,<br>Olympiad System</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            return created;
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
