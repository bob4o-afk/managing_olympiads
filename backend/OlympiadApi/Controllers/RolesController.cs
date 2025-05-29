using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        // Inject the RoleService via constructor
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/roles
        [HttpGet]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleService.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching roles.", error = ex.Message });
            }
        }

        // POST: api/roles
        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            try
            {
                var createdRole = await _roleService.CreateRoleAsync(role);
                if (createdRole == null)
                {
                    return BadRequest(new { message = "RoleName is required." });
                }

                return Ok(new { message = "Role created successfully", role = createdRole });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the role.", error = ex.Message });
            }
        }
        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var deleted = await _roleService.DeleteRoleAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Role not found." });
                }

                return Ok(new { message = "Role deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the role.", error = ex.Message });
            }
        }
    }
}
