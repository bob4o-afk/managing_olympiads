namespace OlympiadApi.Services
{
    public class SmtpSettings
    {
        public required string Server { get; set; }
        public int Port { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
    }
}
