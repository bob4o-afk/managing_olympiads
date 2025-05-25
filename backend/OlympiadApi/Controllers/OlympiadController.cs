using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.Models;
using OlympiadApi.Filters;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OlympiadController : ControllerBase
    {
        private readonly IOlympiadService _olympiadService;

        public OlympiadController(IOlympiadService olympiadService)
        {
            _olympiadService = olympiadService;
        }

        // GET: api/olympiad
        [HttpGet]
        public IActionResult GetAllOlympiads()
        {
            try
            {
                var olympiads = _olympiadService.GetAllOlympiads();
                return Ok(olympiads); // Return all Olympiads
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the Olympiads.", error = ex.Message });
            }
        }

        // GET: api/olympiad/{id}
        [HttpGet("{id}")]
        public IActionResult GetOlympiadById(int id)
        {
            try
            {
                var olympiad = _olympiadService.GetOlympiadById(id);
                if (olympiad == null)
                {
                    return NotFound(new { message = "Olympiad not found." });
                }
                return Ok(olympiad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the Olympiad.", error = ex.Message });
            }
        }

        // POST: api/olympiad
        [HttpPost]
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
        public IActionResult CreateOlympiad([FromBody] Olympiad olympiad)
        {
            try
            {
                if (olympiad == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                _olympiadService.AddOlympiad(olympiad);


                return CreatedAtAction(nameof(GetOlympiadById), new { id = olympiad.OlympiadId }, olympiad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Olympiad.", error = ex.Message });
            }
        }
    }
}
