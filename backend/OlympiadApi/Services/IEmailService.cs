namespace OlympiadApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, string? ccEmail = null);
    }
}
