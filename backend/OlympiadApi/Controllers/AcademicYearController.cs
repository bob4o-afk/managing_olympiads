using OlympiadApi.Services.Interfaces;
using OlympiadApi.Models;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/academic-years")]
    public class AcademicYearController : ControllerBase
    {
        private readonly IAcademicYearService _academicYearService;

        public AcademicYearController(IAcademicYearService academicYearService)
        {
            _academicYearService = academicYearService;
        }

        // GET: api/academic-years
        [HttpGet]
        public async Task<IActionResult> GetAllAcademicYear()
        {
            try
            {
                var academicYears = await _academicYearService.GetAllAcademicYearsAsync();
                return Ok(academicYears);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the academic years.", error = ex.Message });
            }
        }

        // GET: api/academic-years/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAcademicYearById(int id)
        {
            try
            {
                var academicYear = await _academicYearService.GetAcademicYearByIdAsync(id);
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

        // POST: api/academic-years
        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> CreateAcademicYear([FromBody] AcademicYear academicYear)
        {
            try
            {
                if (academicYear == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                await _academicYearService.AddAcademicYearAsync(academicYear.StartYear, academicYear.EndYear);

                return CreatedAtAction(nameof(GetAcademicYearById), new { id = academicYear.AcademicYearId }, academicYear);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the academic year.", error = ex.Message });
            }
        }
    }
}
