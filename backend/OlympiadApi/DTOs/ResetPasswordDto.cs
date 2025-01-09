namespace OlympiadApi.DTOs
{
    public class ResetPasswordDto
    {
        public required string Username { get; set; }
        public required string NewPassword { get; set; }
    }

}
