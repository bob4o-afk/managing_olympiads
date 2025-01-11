using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;
using OlympiadApi.Models;
using OlympiadApi.Services;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        // Inject the RoleService via constructor
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/role
        [HttpGet]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
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

        // POST: api/role
        [HttpPost]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
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
    }
}
