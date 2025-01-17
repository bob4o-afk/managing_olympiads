
namespace OlympiadApi.DTOs
{
    public class SendEmailWithDocumentDto
    {
        public required string ToEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required IFormFile Document { get; set; }
        public string? CcEmail { get; set; }
    }
}
