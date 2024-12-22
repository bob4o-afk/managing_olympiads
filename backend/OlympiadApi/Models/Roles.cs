namespace OlympiadApi.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public Dictionary<string, object>? Permissions { get; set; }
    }
}
