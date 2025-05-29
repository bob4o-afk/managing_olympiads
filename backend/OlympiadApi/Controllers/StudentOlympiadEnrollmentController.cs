using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Controllers
{
    [Route("api/student-olympiad-enrollments")]
    [ApiController]
    public class StudentOlympiadEnrollmentController : ControllerBase
    {
        private readonly IStudentOlympiadEnrollmentService _service;

        public StudentOlympiadEnrollmentController(IStudentOlympiadEnrollmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            try
            {
                var enrollments = await _service.GetAllEnrollmentsAsync();
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching enrollments.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            try
            {
                var enrollment = await _service.GetEnrollmentByIdAsync(id);
                if (enrollment == null)
                {
                    return NotFound();
                }
                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the enrollment.", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [RoleAuthorize("Admin", "Student")]
        public async Task<IActionResult> GetEnrollmentsByUserId(int userId)
        {
            try
            {
                var enrollments = await _service.GetEnrollmentsByUserIdAsync(userId);
                if (enrollments == null || enrollments.Count == 0)
                {
                    return NotFound(new { Message = "No enrollments found for this user." });
                }
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching user enrollments.", error = ex.Message });
            }
        }

        [HttpPost]
        [RoleAuthorize("Admin", "Student")]
        public async Task<IActionResult> CreateEnrollment([FromBody] StudentOlympiadEnrollment enrollment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdEnrollment = await _service.CreateEnrollmentAsync(enrollment);
                return CreatedAtAction(nameof(GetEnrollmentById), new { id = createdEnrollment.EnrollmentId }, createdEnrollment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the enrollment.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] StudentOlympiadEnrollment updatedEnrollment)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the enrollment.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            try
            {
                var success = await _service.DeleteEnrollmentAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the enrollment.", error = ex.Message });
            }
        }
    }
}
