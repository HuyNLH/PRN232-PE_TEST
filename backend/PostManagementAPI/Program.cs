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
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

