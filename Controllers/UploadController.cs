// ============================================================
// Controller: UploadController
// Course: Application security
// Student: Tanisha, Kridhay Makwana, Manisha Bhatia
//
// Description:
// This controller handles file upload functionality.
// It validates file type, size, and securely stores files.
// Metadata is saved in the database.
//
// Security Features:
// - File type validation (PDF only)
// - File size restriction
// - Input validation
// - Parameterized queries
// - Exception handling
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CareerFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public UploadController(DatabaseHelper db)
        {
            _db = db;
        }

        // ============================================================
        // Method: UploadResume
        // Description:
        // Uploads a resume file for a user after validating file type,
        // size, and user input. Saves file to server and metadata to DB.
        //
        // Input:
        // file (IFormFile) – uploaded file
        // userId (int) – ID of user
        //
        // Returns:
        // Success or error message
        // ============================================================
        [HttpPost("resume")]
        public async Task<IActionResult> UploadResume(IFormFile file, int userId)
        {
            // Validate user ID
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            // Validate file existence
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Validate file type (PDF only)
            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size exceeds 5MB limit.");
            }

            try
            {
                //  Ensure Uploads folder exists
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate unique file name (prevents overwrite)
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(folderPath, uniqueFileName);

                // Save file to server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //  Save metadata to database
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Uploads 
                                     (FileName, FilePath, FileType, FileSize, UserID) 
                                     VALUES (@name, @path, @type, @size, @userId)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", uniqueFileName);
                    cmd.Parameters.AddWithValue("@path", filePath);
                    cmd.Parameters.AddWithValue("@type", file.ContentType);
                    cmd.Parameters.AddWithValue("@size", file.Length);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    cmd.ExecuteNonQuery();
                }

                return Ok("File uploaded successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred during file upload.");
            }
        }
    }
}