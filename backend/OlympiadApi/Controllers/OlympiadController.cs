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
        public async Task<IActionResult> GetAllOlympiadsAsync()
        {
            try
            {
                var olympiads = await _olympiadService.GetAllOlympiadsAsync();
                return Ok(olympiads); // Return all Olympiads
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the Olympiads.", error = ex.Message });
            }
        }

        // GET: api/olympiad/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOlympiadByIdAsync(int id)
        {
            try
            {
                var olympiad = await _olympiadService.GetOlympiadByIdAsync(id);
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
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> CreateOlympiadAsync([FromBody] Olympiad olympiad)
        {
            try
            {
                if (olympiad == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                await _olympiadService.AddOlympiadAsync(olympiad);


                return CreatedAtAction(nameof(GetOlympiadByIdAsync), new { id = olympiad.OlympiadId }, olympiad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Olympiad.", error = ex.Message });
            }
        }
    }
}
