using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace OlympiadApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
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
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
                await client.AuthenticateAsync(_smtpSettings.User, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
