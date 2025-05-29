using Microsoft.Extensions.Options;
using MimeKit;
using OlympiadApi.DTos;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly Func<ISmtpClient> _smtpClientFactory;

        public EmailService(IOptions<SmtpSettings> smtpSettings, Func<ISmtpClient> smtpClientFactory)
        {
            _smtpSettings = smtpSettings.Value;
            _smtpClientFactory = smtpClientFactory;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string? ccEmail = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Olympiad System", _smtpSettings.User));
            message.To.Add(new MailboxAddress("", toEmail));
            if (!string.IsNullOrEmpty(ccEmail))
            {
                message.Cc.Add(new MailboxAddress("", ccEmail));
            }
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            
            message.Body = bodyBuilder.ToMessageBody();

            using var client = _smtpClientFactory();
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
            await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, IFormFile document, string? ccEmail = null)
        {
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await document.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Create the email message
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Olympiad System", _smtpSettings.User));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            if (!string.IsNullOrEmpty(ccEmail))
            {
                emailMessage.Cc.Add(new MailboxAddress("", ccEmail));
            }
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { TextBody = body };
            bodyBuilder.Attachments.Add(document.FileName, fileBytes, MimeKit.ContentType.Parse(document.ContentType));

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = _smtpClientFactory();
            await smtpClient.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
            await smtpClient.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);
            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
