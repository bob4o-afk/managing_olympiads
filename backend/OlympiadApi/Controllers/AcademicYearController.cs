using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Data;
using OlympiadApi.Models;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicYearController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext via constructor
        public AcademicYearController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/academicyear
        [HttpGet]
        public IActionResult GetAllAcademicYear()
        {
            try
            {
                var academicYear = _context.AcademicYear.ToList();
                return Ok(academicYear); // Return all academic years
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the academic years.", error = ex.Message });
            }
        }

        // GET: api/academicyear/{id}
        [HttpGet("{id}")]
        public IActionResult GetAcademicYearById(int id)
        {
            try
            {
                var academicYear = _context.AcademicYear.FirstOrDefault(ay => ay.AcademicYearId == id);
                if (academicYear == null)
                {
                    return NotFound(new { message = "Academic Year not found." });
                }
                return Ok(academicYear);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the academic year.", error = ex.Message });
            }
        }

        // POST: api/academicyear
        [HttpPost]
        public IActionResult CreateAcademicYear([FromBody] AcademicYear academicYear)
        {
            try
            {
                if (academicYear == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                _context.AcademicYear.Add(academicYear);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetAcademicYearById), new { id = academicYear.AcademicYearId }, academicYear);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the academic year.", error = ex.Message });
            }
        }
    }
}
