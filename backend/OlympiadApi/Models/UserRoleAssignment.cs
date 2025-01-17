using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlympiadApi.Models
{
    public class UserRoleAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentId { get; set; }

        public required int UserId { get; set; }

        public required int RoleId { get; set; }

        public required DateTime AssignedAt { get; set; }

        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
