// ============================================================
// Class: DatabaseHelper
// Course: Secure Client-Server Application
// Student: Manisha Bhatia
//
// Description:
// This class is responsible for managing database connections.
// It retrieves the connection string from configuration and
// provides a method to create SQL Server connections.
//
// Features:
// - Centralized database connection handling
// - Uses configuration-based connection string
// - Supports secure and reusable database access
// ============================================================

using Microsoft.Data.SqlClient;

namespace CareerFlowAPI
{
    public class DatabaseHelper
    {
        // ============================================================
        // Variable: _connectionString
        // Description:
        // Stores the database connection string retrieved from
        // appsettings.json configuration file.
        // ============================================================
        private readonly string _connectionString;

        // ============================================================
        // Constructor: DatabaseHelper
        // Description:
        // Initializes the connection string using dependency injection.
        //
        // Input:
        // configuration (IConfiguration) – application configuration
        // ============================================================
        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ============================================================
        // Method: GetConnection
        // Description:
        // Creates and returns a new SQL Server connection object.
        //
        // Returns:
        // SqlConnection – database connection instance
        // ============================================================
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}