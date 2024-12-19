using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OlympiadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext via constructor
        public OlympiadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/olympiad
        [HttpGet]
        public IActionResult GetAllOlympiads()
        {
            try
            {
                var olympiads = _context.Olympiads.ToList();
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
                var olympiad = _context.Olympiads.FirstOrDefault(o => o.OlympiadId == id);
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
        public IActionResult CreateOlympiad([FromBody] Olympiad olympiad)
        {
            try
            {
                if (olympiad == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                _context.Olympiads.Add(olympiad);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetOlympiadById), new { id = olympiad.OlympiadId }, olympiad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Olympiad.", error = ex.Message });
            }
        }
    }
}
