    namespace OlympiadApi.Models;

    public class EmailRequest
    {
        public required string ToEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string CcEmail { get; set; }
    }