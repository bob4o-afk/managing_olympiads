using System.ComponentModel.DataAnnotations;

namespace OlympiadApi.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
