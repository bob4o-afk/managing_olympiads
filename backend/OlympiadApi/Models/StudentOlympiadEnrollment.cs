using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlympiadApi.Models
{
    public class StudentOlympiadEnrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentId { get; set; }

        public required int UserId { get; set; }

        public required int OlympiadId { get; set; }

        public required int AcademicYearId { get; set; }

        public required string EnrollmentStatus { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public Dictionary<string, object>? StatusHistory { get; set; }

        public int? Score { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Olympiad? Olympiad { get; set; }
        public AcademicYear? AcademicYear { get; set; }
    }
}
