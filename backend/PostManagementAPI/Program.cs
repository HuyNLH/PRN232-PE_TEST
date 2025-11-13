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
        Description = "REST API for managing movies with CRUD operations, search, filter, and Cloudinary image upload."
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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Management API v1");
    options.RoutePrefix = "swagger"; // Access at /swagger
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible for testing
public partial class Program { }

