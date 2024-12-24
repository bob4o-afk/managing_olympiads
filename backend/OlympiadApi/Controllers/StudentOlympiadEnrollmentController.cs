using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Models;
using OlympiadApi.Services;

namespace OlympiadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentOlympiadEnrollmentController : ControllerBase
    {
        private readonly StudentOlympiadEnrollmentService _service;

        public StudentOlympiadEnrollmentController(StudentOlympiadEnrollmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _service.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            var enrollment = await _service.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return Ok(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] StudentOlympiadEnrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEnrollment = await _service.CreateEnrollmentAsync(enrollment);
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = createdEnrollment.EnrollmentId }, createdEnrollment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] StudentOlympiadEnrollment updatedEnrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _service.UpdateEnrollmentAsync(id, updatedEnrollment);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var success = await _service.DeleteEnrollmentAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
