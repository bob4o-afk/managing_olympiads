using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Controllers;
using OlympiadApi.DTOs;
using OlympiadApi.Services.Interfaces;
using System.Text;

namespace OlympiadApi.Tests.Controllers
{
    public class EmailControllerTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly EmailController _controller;

        public EmailControllerTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _controller = new EmailController(_emailServiceMock.Object);
        }

        [Fact]
        public async Task SendEmail_ReturnsOk_WhenCalled()
        {
            var request = new EmailRequest
            {
                ToEmail = "test@example.com",
                Subject = "Test Subject",
                Body = "Test Body",
                CcEmail = "cc@example.com"
            };

            _emailServiceMock
                .Setup(s => s.SendEmailAsync(request.ToEmail, request.Subject, request.Body, request.CcEmail))
                .Returns(Task.CompletedTask);

            var result = await _controller.SendEmail(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email sent successfully.", ok.Value);
        }

        [Fact]
        public async Task SendDocumentAsync_ReturnsOk_WhenDocumentIsValid()
        {
            var content = "This is a test file.";
            var fileName = "test.txt";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var file = new FormFile(stream, 0, stream.Length, "Document", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var request = new SendEmailWithDocumentDto
            {
                ToEmail = "recipient@example.com",
                Subject = "Doc Subject",
                Body = "Body",
                Document = file,
                CcEmail = "cc@example.com"
            };

            _emailServiceMock
                .Setup(s => s.SendEmailWithAttachmentAsync(
                    request.ToEmail,
                    request.Subject,
                    request.Body,
                    request.Document,
                    request.CcEmail))
                .Returns(Task.CompletedTask);

            var result = await _controller.SendDocumentAsync(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email with document sent successfully.", ok.Value);
        }

        [Fact]
        public async Task SendDocumentAsync_ReturnsBadRequest_WhenFileIsMissing()
        {
            var request = new SendEmailWithDocumentDto
            {
                ToEmail = "recipient@example.com",
                Subject = "Doc Subject",
                Body = "Body",
                Document = null!,
                CcEmail = "cc@example.com"
            };

            var result = await _controller.SendDocumentAsync(request);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Please provide a valid document to attach.", bad.Value);
        }

        [Fact]
        public async Task SendDocumentAsync_ReturnsBadRequest_WhenFileIsEmpty()
        {
            var file = new FormFile(Stream.Null, 0, 0, "Document", "empty.txt");

            var request = new SendEmailWithDocumentDto
            {
                ToEmail = "recipient@example.com",
                Subject = "Empty Doc",
                Body = "Body",
                Document = file,
                CcEmail = null
            };

            var result = await _controller.SendDocumentAsync(request);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Please provide a valid document to attach.", bad.Value);
        }
    }
}
