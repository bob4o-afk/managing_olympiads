using Microsoft.AspNetCore.Mvc;
using MySQLRandomNumberApp.Data;

namespace MySQLRandomNumberApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NumberController : ControllerBase
    {
        private readonly DatabaseHelper _databaseHelper;

        // Inject DatabaseHelper via constructor
        public NumberController(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        // Handle GET requests to /number/get?number=your_number
        [HttpGet("get")]
        public IActionResult InsertRandomNumber([FromQuery] int number)
        {
            try
            {
                // Insert the number into the database
                _databaseHelper.InsertRandomNumber(number);

                // Return success response
                return Ok(new { message = "Random number saved successfully.", number });
            }
            catch (Exception ex)
            {
                // Return error response if something goes wrong
                return StatusCode(500, new { message = "An error occurred while saving the number.", error = ex.Message });
            }
        }
    }
}
