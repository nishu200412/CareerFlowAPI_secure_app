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

        // ============================================================
        // Method: Apply
        // Description:
        // Allows a user to apply for a job.
        // Includes validation and secure database insertion.
        //
        // Input:
        // userId (int) – ID of user
        // jobId (int) – ID of job
        //
        // Returns:
        // Success or error message
        // ============================================================
        [HttpPost("apply")]
        public IActionResult Apply(int userId, int jobId)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // ============================
                // Check if User exists
                // ============================
                string checkUser = "SELECT COUNT(*) FROM Users WHERE UserID = @userId";
                SqlCommand userCmd = new SqlCommand(checkUser, conn);
                userCmd.Parameters.AddWithValue("@userId", userId);

                if ((int)userCmd.ExecuteScalar() == 0)
                {
                    return BadRequest("User does not exist");
                }

                // ============================
                // Check if Job exists
                // ============================
                string checkJob = "SELECT COUNT(*) FROM Jobs WHERE JobID = @jobId";
                SqlCommand jobCmd = new SqlCommand(checkJob, conn);
                jobCmd.Parameters.AddWithValue("@jobId", jobId);

                if ((int)jobCmd.ExecuteScalar() == 0)
                {
                    return BadRequest("Job does not exist");
                }

                // ============================
                // Insert Application
                // ============================
                string query = "INSERT INTO Applications (UserID, JobID, Status) VALUES (@userId, @jobId, 'Pending')";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@jobId", jobId);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    return Ok("Application submitted successfully");
                }
                else
                {
                    return BadRequest("Application failed");
                }
            }
        }   
    }
}