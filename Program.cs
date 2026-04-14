// ============================================================
// File: Program.cs
// Course: Application security
// Student: Manisha Bhatia, Tanisha, Kridhay Makwana
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
// - CORS enabled for UI communication
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

            // ============================================================
            // Add CORS policy (allows UI to call API)
            // ============================================================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

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

            // ============================================================
            // Enable CORS
            // ============================================================
            app.UseCors("AllowAll");

            app.UseAuthorization();

            // Map controller routes
            app.MapControllers();

            // Start the application
            app.Run();
        }
    }
}