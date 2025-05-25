using OlympiadApi.Services.Interfaces;
using OlympiadApi.Models;
using Microsoft.AspNetCore.Mvc;
using OlympiadApi.Filters;

namespace OlympiadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicYearController : ControllerBase
    {
        private readonly IAcademicYearService _academicYearService;

        public AcademicYearController(IAcademicYearService academicYearService)
        {
            _academicYearService = academicYearService;
        }

        // GET: api/academicyear
        [HttpGet]
        public IActionResult GetAllAcademicYear()
        {
            try
            {
                var academicYears = _academicYearService.GetAllAcademicYears();
                return Ok(academicYears);
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
                var academicYear = _academicYearService.GetAcademicYearById(id);
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
        [ServiceFilter(typeof(AdminRoleAuthorizeAttribute))]
        public IActionResult CreateAcademicYear([FromBody] AcademicYear academicYear)
        {
            try
            {
                if (academicYear == null)
                {
                    return BadRequest(new { message = "Invalid data." });
                }

                _academicYearService.AddAcademicYear(academicYear.StartYear, academicYear.EndYear);

                return CreatedAtAction(nameof(GetAcademicYearById), new { id = academicYear.AcademicYearId }, academicYear);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the academic year.", error = ex.Message });
            }
        }
    }
}
