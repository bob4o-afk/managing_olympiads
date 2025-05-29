namespace OlympiadApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, string? ccEmail = null);
        Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, IFormFile document, string? ccEmail = null);
    }
}
