using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlympiadApi.Models
{
    public class AcademicYear
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AcademicYearId { get; set; }
        public required int StartYear { get; set; }
        public required int EndYear { get; set; }

    }
}
