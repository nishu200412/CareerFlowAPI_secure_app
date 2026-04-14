// ============================================================
// Controller: JobsController
// Course: Application Security
// Student: Manisha Bhatia
//
// Description:
// This controller handles job-related operations including:
// - Searching jobs
// - Applying for jobs
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
    public class JobsController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public JobsController(DatabaseHelper db)
        {
            _db = db;
        }

        // ============================================================
        // Method: Search
        // Description:
        // Allows users to search for jobs using a keyword.
        // Includes input validation and secure query execution.
        //
        // Input:
        // keyword (string) – search term
        //
        // Returns:
        // List of matching jobs or error message
        // ============================================================
        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            // Validation: Check for empty input
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Search keyword is required.");
            }

            // This will limit keyword length
            if (keyword.Length > 50)
            {
                return BadRequest("Keyword too long.");
            }

            var results = new List<object>();

            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT JobID, Title, Location FROM Jobs WHERE Title LIKE @keyword";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword.Trim() + "%");

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        results.Add(new
                        {
                            JobID = reader["JobID"],
                            Title = reader["Title"],
                            Location = reader["Location"]
                        });
                    }
                }

                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error occurred while searching jobs.");
            }
        }

    }
}