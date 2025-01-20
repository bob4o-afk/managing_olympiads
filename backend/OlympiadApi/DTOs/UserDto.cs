namespace OlympiadApi.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? Gender { get; set; }
        public Dictionary<string, object>? PersonalSettings { get; set; }
        public Dictionary<string, object>? Notifications { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
