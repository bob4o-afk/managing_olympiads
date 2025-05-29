using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Controllers
{
    [Route("api/user-role-assignments")]
    [ApiController]
    public class UserRoleAssignmentController : ControllerBase
    {
        private readonly IUserRoleAssignmentService _service;

        public UserRoleAssignmentController(IUserRoleAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [RoleAuthorize("Admin", "Student")]
        public async Task<IActionResult> GetAllAssignments()
        {
            var assignments = await _service.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        [HttpGet("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _service.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return Ok(assignment);
        }

        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> CreateAssignment([FromBody] UserRoleAssignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAssignment = await _service.CreateAssignmentAsync(assignment);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = createdAssignment.AssignmentId }, createdAssignment);
        }

        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var success = await _service.DeleteAssignmentAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
