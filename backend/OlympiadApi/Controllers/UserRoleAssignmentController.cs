using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;
using OlympiadApi.Models;
using OlympiadApi.Services;

namespace OlympiadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleAssignmentController : ControllerBase
    {
        private readonly UserRoleAssignmentService _service;

        public UserRoleAssignmentController(UserRoleAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        //Check for studens only
        public async Task<IActionResult> GetAllAssignments()
        {
            var assignments = await _service.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
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
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
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
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
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
