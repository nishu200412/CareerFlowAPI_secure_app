using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CareerFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public AuthController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // 🔹 Input validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            // Optional: basic length check
            if (username.Length < 3 || password.Length < 3)
            {
                return BadRequest("Invalid input length.");
            }

            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT Role FROM Users WHERE Username = @username AND PasswordHash = @password";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username.Trim());
                    cmd.Parameters.AddWithValue("@password", password.Trim());

                    var role = cmd.ExecuteScalar();

                    if (role != null)
                    {
                        return Ok(new { message = "Login successful", role = role.ToString() });
                    }
                    else
                    {
                        return Unauthorized("Invalid credentials.");
                    }
                }
            }
            catch (Exception)
            {
                //  Generic error (do NOT expose DB details)
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}