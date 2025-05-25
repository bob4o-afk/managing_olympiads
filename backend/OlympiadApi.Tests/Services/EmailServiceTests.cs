using Moq;
using Microsoft.Extensions.Options;
using MimeKit;
using OlympiadApi.Services;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.DTos;
using Microsoft.AspNetCore.Http;
using System.Text;

public class EmailServiceTests
{
    private EmailService CreateService(Mock<ISmtpClient> smtpClientMock, out Mock<ISmtpClient> clientMock)
    {
        clientMock = smtpClientMock;
        var smtpSettings = Options.Create(new SmtpSettings
        {
            Server = "smtp.test.com",
            Port = 587,
            User = "sender@test.com",
            Password = "password"
        });

        return new EmailService(smtpSettings, () => smtpClientMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_SendsToWithAndWithoutCc()
    {
        var mockClient = new Mock<ISmtpClient>();
        var service = CreateService(mockClient, out _);

        await service.SendEmailAsync("to@example.com", "Subject", "<p>Body</p>", "cc@example.com");

        await service.SendEmailAsync("to@example.com", "Subject", "<p>Body</p>");

        mockClient.Verify(c => c.ConnectAsync("smtp.test.com", 587, false), Times.Exactly(2));
        mockClient.Verify(c => c.AuthenticateAsync("sender@test.com", "password"), Times.Exactly(2));
        mockClient.Verify(c => c.SendAsync(It.Is<MimeMessage>(m =>
            m.Subject == "Subject" &&
            m.To.Mailboxes.Any(mb => mb.Address == "to@example.com") &&
            m.HtmlBody.Contains("Body"))), Times.Exactly(2));
        mockClient.Verify(c => c.DisconnectAsync(true), Times.Exactly(2));
    }

    [Fact]
    public async Task SendEmailWithAttachmentAsync_SendsEmailWithAttachment()
    {
        var mockClient = new Mock<ISmtpClient>();
        var service = CreateService(mockClient, out _);

        var fileMock = new Mock<IFormFile>();
        var content = "Test file content";
        var fileName = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("text/plain");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((target, token) =>
                {
                    stream.CopyTo(target);
                    return Task.CompletedTask;
                });

        await service.SendEmailWithAttachmentAsync("to@example.com", "Attachment", "Here is your file", fileMock.Object, "cc@example.com");

        stream.Position = 0;
        await service.SendEmailWithAttachmentAsync("to@example.com", "Attachment", "Here is your file", fileMock.Object);

        mockClient.Verify(c => c.ConnectAsync("smtp.test.com", 587, false), Times.Exactly(2));
        mockClient.Verify(c => c.AuthenticateAsync("sender@test.com", "password"), Times.Exactly(2));
        mockClient.Verify(c => c.SendAsync(It.Is<MimeMessage>(m =>
            m.Subject == "Attachment" &&
            m.To.Mailboxes.Any(mb => mb.Address == "to@example.com") &&
            m.Body is Multipart &&
            m.Body.ToString().Contains("Here is your file") &&
            m.Attachments.Any(a => a.ContentDisposition != null && a.ContentDisposition.FileName == "test.txt"))), Times.Exactly(2));
        mockClient.Verify(c => c.DisconnectAsync(true), Times.Exactly(2));
    }
}
