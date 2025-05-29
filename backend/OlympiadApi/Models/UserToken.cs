using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlympiadApi.Models
{
    public class UserToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserTokenId { get; set; }
        public int UserId { get; set; }
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }

        public User? User { get; set; }
    }
}
