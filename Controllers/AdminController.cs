// ============================================================
// Controller: AdminController
// Course: Secure Client-Server Application
// Student: Manisha Bhatia
//
// Description:
// This controller provides admin-only access to view all
// job applications. It demonstrates role-based access control.
//
// Security Features:
// - Role-based authorization (Admin only)
// - Input validation
// - Parameterized query usage
// - Exception handling
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CareerFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public AdminController(DatabaseHelper db)
        {
            _db = db;
        }

        // ============================================================
        // Method: GetAllApplications
        // Description:
        // Allows admin users to view all job applications.
        //
        // Input:
        // role (string) – user role (must be "Admin")
        //
        // Returns:
        // List of applications or unauthorized message
        // ============================================================
        [HttpGet("applications")]
        public IActionResult GetAllApplications(string role)
        {
            // Validate role input
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Role is required.");
            }

            // Normalize role input (avoid case issues)
            if (!role.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Access denied. Admin only.");
            }

            var results = new List<object>();

            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT U.Username, J.Title, A.Status
                                     FROM Applications A
                                     JOIN Users U ON A.UserID = U.UserID
                                     JOIN Jobs J ON A.JobID = J.JobID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        results.Add(new
                        {
                            Username = reader["Username"],
                            JobTitle = reader["Title"],
                            Status = reader["Status"]
                        });
                    }
                }

                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error occurred while retrieving applications.");
            }
        }
    }
}