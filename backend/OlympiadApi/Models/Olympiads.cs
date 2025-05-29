using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlympiadApi.Models
{
    public class Olympiad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OlympiadId { get; set; }
        public required string Subject { get; set; }
        public string? Description { get; set; }
        public required DateTime DateOfOlympiad { get; set; }
        public required string Round { get; set; }
        public required string Location { get; set; }
        public DateTime? StartTime { get; set; }
        public required int ClassNumber { get; set; }

        // Foreign Key to AcademicYear
        public int AcademicYearId { get; set; }

        // Navigation property to AcademicYear
        public AcademicYear? AcademicYear { get; set; }

    }
}
