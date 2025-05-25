using MimeKit;

namespace OlympiadApi.Services.Interfaces
{
    public interface ISmtpClient : IDisposable
    {
        Task ConnectAsync(string host, int port, bool useSsl);
        Task AuthenticateAsync(string userName, string password);
        Task SendAsync(MimeMessage message);
        Task DisconnectAsync(bool quit);
    }
}
