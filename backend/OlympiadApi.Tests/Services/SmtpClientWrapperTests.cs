using MimeKit;
using OlympiadApi.Services;

public class SmtpClientWrapperTests
{
    [Fact]
    public async Task SmtpClientWrapper_CoversAllMethods()
    {
        var client = new SmtpClientWrapper();
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Test", "sender@example.com"));
        message.To.Add(new MailboxAddress("Test", "recipient@example.com"));
        message.Subject = "Test Email";
        message.Body = new BodyBuilder { TextBody = "Body content" }.ToMessageBody();

        await client.ConnectAsync("smtp.gmail.com", 25, false);

        // these will throw error cuz it isnt authenticated
        await Assert.ThrowsAsync<MailKit.Security.AuthenticationException>(() =>
            client.AuthenticateAsync("user", "pass"));

        await Assert.ThrowsAsync<MailKit.ServiceNotAuthenticatedException>(() =>
            client.SendAsync(message));

        await client.DisconnectAsync(true);

        client.Dispose();
    }
}
