using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PostManagementAPI.Data;
using PostManagementAPI.Configuration;
using PostManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movie Management API",
        Version = "v1",
        Description = @"A comprehensive REST API for managing movies with advanced features:
        
**Features:**
- Full CRUD operations for movies
- Search movies by title
- Filter by genre
- Sort by rating (ascending/descending)
- Pagination support
- Image upload via file or URL (Cloudinary integration)
- Automatic database seeding with sample data

**Quick Start:**
1. Use GET /api/movies to retrieve all movies
2. Use POST /api/movies to create a new movie
3. Use PUT /api/movies/{id} to update a movie
4. Use DELETE /api/movies/{id} to remove a movie
5. Check /health for API status",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@movieapi.com"
        }
    });
    
    options.EnableAnnotations();
    
    // Support for file uploads in Swagger UI
    options.OperationFilter<FileUploadOperationFilter>();
    
    // Add schema examples
    options.SchemaFilter<MovieSchemaFilter>();
});

// Configure PostgreSQL Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Configure Cloudinary settings
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

// Register Image Upload Service
builder.Services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();

// Register Database Seeder
builder.Services.AddScoped<DatabaseSeeder>();

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()  // Allow all origins (including Vercel)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

Console.WriteLine("✅ CORS configured: AllowAnyOrigin enabled for Vercel");

// Auto-migrate database on startup (Production)
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            Console.WriteLine("🔄 [PRODUCTION] Running database migrations...");
            Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("Default")?.Substring(0, 50)}...");
            
            await dbContext.Database.MigrateAsync();
            
            Console.WriteLine("✅ [PRODUCTION] Database migrations completed successfully!");
            
            // Seed data if database is empty
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
            Console.WriteLine("✅ [PRODUCTION] Database seeding completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [PRODUCTION] Database migration error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to prevent app from starting with broken DB
        }
    }
}

// Handle --seed command line argument
if (args.Contains("--seed"))
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
        Console.WriteLine("Database seeding completed successfully!");
        return; // Exit after seeding
    }
}

// Configure the HTTP request pipeline.
// Enable Swagger in all environments (including Production for Render)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Post Management API v1");
    options.RoutePrefix = "swagger"; // Access at /swagger
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Health endpoint - simple OK response
app.MapGet("/health", () => Results.Ok("OK"))
    .WithName("HealthCheck")
    .WithTags("Health")
    .Produces<string>(StatusCodes.Status200OK);

// Root endpoint for API info
app.MapGet("/", () => new { 
    status = "ok", 
    message = "Movie Management API is running",
    timestamp = DateTime.UtcNow,
    endpoints = new[] { "/swagger", "/health", "/api/movies" }
});

// Manual seed endpoint (for debugging)
app.MapPost("/api/seed", async (IServiceProvider serviceProvider) =>
{
    using var scope = serviceProvider.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
    return Results.Ok(new { message = "Database seeded successfully!" });
})
.WithName("SeedDatabase")
.WithTags("Database");

app.Run();

// Make Program class accessible for testing
public partial class Program { }

