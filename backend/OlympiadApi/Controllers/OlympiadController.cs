using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Services.Interfaces;
using OlympiadApi.Models;
using OlympiadApi.Filters;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/olympiads")]
    public class OlympiadController : ControllerBase
    {
        private readonly IOlympiadService _olympiadService;

        public OlympiadController(IOlympiadService olympiadService)
        {
            _olympiadService = olympiadService;
        }

        // GET: api/olympiads
        [HttpGet]
        public async Task<IActionResult> GetAllOlympiads()
        {
            try
            {
                var olympiads = await _olympiadService.GetAllOlympiadsAsync();
                return Ok(olympiads);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the Olympiads.", error = ex.Message });
            }
        }

        // GET: api/olympiads/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOlympiadById(int id)
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

        // POST: api/olympiads
        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> CreateOlympiad([FromBody] Olympiad olympiad)
        {
            try
            {
                if (olympiad == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                var createdOlympiad = await _olympiadService.AddOlympiadAsync(olympiad);

                return CreatedAtAction(nameof(GetOlympiadById), new { id = createdOlympiad.OlympiadId }, createdOlympiad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Olympiad.", error = ex.Message });
            }
        }
    }
}
