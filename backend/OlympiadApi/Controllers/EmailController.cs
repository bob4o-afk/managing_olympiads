using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.DTOs;
using OlympiadApi.Filters;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        //TO DO: check - maybe i need admin or email
        //here should be made a checking for group - so only with the elsys emails

        [HttpPost("send")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Body, request.CcEmail);
            return Ok("Email sent successfully.");
        }

        [HttpPost("send-document")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> SendDocumentAsync([FromForm] SendEmailWithDocumentDto request)
        {
            if (request.Document == null || request.Document.Length == 0)
            {
                return BadRequest("Please provide a valid document to attach.");
            }

            await _emailService.SendEmailWithAttachmentAsync(request.ToEmail, request.Subject, request.Body, request.Document, request.CcEmail);
            return Ok("Email with document sent successfully.");
        }

    }
}
