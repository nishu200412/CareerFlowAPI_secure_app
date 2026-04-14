// ============================================================
// Controller: AuthController
// Course: Application Security
// Student: Manisha Bhatia, Tanisha, Kridhay Makwana
//
// Description:
// This controller handles user authentication.
// It provides a login API that validates user credentials
// and returns the user role upon successful login.
//
// Security Features:
// - Input validation
// - Parameterized SQL queries (prevents SQL injection)
// - Exception handling
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CareerFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        // ============================================================
        // Constructor: AuthController
        // Description:
        // Initializes the controller with database helper dependency.
        //
        // Input:
        // db (DatabaseHelper) – database connection handler
        // ============================================================
        public AuthController(DatabaseHelper db)
        {
            _db = db;
        }

        // ============================================================
        // Method: Login
        // Description:
        // Authenticates a user by checking username and password
        // against the database. Returns user role if successful.
        //
        // Input:
        // username (string) – user login name
        // password (string) – user password
        //
        // Returns:
        // Success message with role OR error message
        // ============================================================
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // Input validation: check empty values
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            // Basic length validation
            if (username.Length < 3 || password.Length < 3)
            {
                return BadRequest("Invalid input length.");
            }
            // Log attempt
            Console.WriteLine("User login attempted: " + username);

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
                        // Log success
                        Console.WriteLine("Login success for: " + username);

                        return Ok(new { message = "Login successful", role = role.ToString() });
                    }
                    else
                    {
                        // Log failure
                        Console.WriteLine("Login failed for: " + username);

                        return Unauthorized("Invalid credentials.");
                    }
                }
            }
            catch (Exception)
            {
                // Do not expose internal errors
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}