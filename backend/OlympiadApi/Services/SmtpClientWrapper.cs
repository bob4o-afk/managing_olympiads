using MimeKit;
using MailKit.Net.Smtp;

namespace OlympiadApi.Services
{
    public class SmtpClientWrapper : Interfaces.ISmtpClient
    {
        private readonly SmtpClient _client = new();

        public async Task ConnectAsync(string host, int port, bool useSsl) => await _client.ConnectAsync(host, port, useSsl);
        public async Task AuthenticateAsync(string userName, string password) => await _client.AuthenticateAsync(userName, password);
        public async Task SendAsync(MimeMessage message) => await _client.SendAsync(message);
        public async Task DisconnectAsync(bool quit) => await _client.DisconnectAsync(quit);
        public void Dispose() => _client.Dispose();
    }
}
