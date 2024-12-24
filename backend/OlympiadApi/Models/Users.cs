using System.ComponentModel.DataAnnotations.Schema;


namespace OlympiadApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required int AcademicYearId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Gender { get; set; }
        public bool EmailVerified { get; set; } = false;

        [Column(TypeName = "jsonb")]
        public Dictionary<string, object>? PersonalSettings { get; set; }

        [Column(TypeName = "jsonb")]
        public Dictionary<string, object>? Notifications { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Navigation property for AcademicYear
        public AcademicYear? AcademicYear { get; set; }
    }
}
