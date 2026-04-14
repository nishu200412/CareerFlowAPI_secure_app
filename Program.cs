// ============================================================
// File: Program.cs
// Course: Secure Client-Server Application
// Student: Manisha Bhatia
//
// Description:
// This is the main entry point of the CareerFlowAPI application.
// It configures and starts the ASP.NET Core Web API.
//
// Responsibilities:
// - Initializes application builder
// - Registers services (Controllers, Swagger, DatabaseHelper)
// - Configures middleware pipeline
// - Enables API endpoints and routing
//
// Features:
// - Dependency Injection setup
// - Swagger API documentation support
// - HTTPS redirection
// - Controller-based routing
// ============================================================

namespace CareerFlowAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================================================
            // Register services for dependency injection
            // ============================================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register DatabaseHelper for database access
            builder.Services.AddScoped<DatabaseHelper>();

            var app = builder.Build();

            // ============================================================
            // Configure middleware pipeline
            // ============================================================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Map controller routes
            app.MapControllers();

            // Start the application
            app.Run();
        }
    }
}