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
        Title = "Post Management API",
        Version = "v1",
        Description = "A REST API for managing posts with image upload support (Cloudinary integration)",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@postmanagement.com"
        }
    });
    
    options.EnableAnnotations();
    
    // Support for file uploads in Swagger UI
    options.OperationFilter<FileUploadOperationFilter>();
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
            Console.WriteLine("Running database migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migrations completed!");
            
            // Seed data if database is empty
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
            Console.WriteLine("Database seeding completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration error: {ex.Message}");
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

// Health check endpoint
app.MapGet("/", () => new { 
    status = "ok", 
    message = "Post Management API is running",
    timestamp = DateTime.UtcNow 
});

app.Run();

// Make Program class accessible for testing
public partial class Program { }

