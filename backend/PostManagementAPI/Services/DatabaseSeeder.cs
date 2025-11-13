using PostManagementAPI.Data;
using PostManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PostManagementAPI.Services
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Ensure database is created
                await _context.Database.MigrateAsync();

                // Delete all existing movies first
                var existingMovies = await _context.Movies.ToListAsync();
                if (existingMovies.Any())
                {
                    _logger.LogInformation($"Deleting {existingMovies.Count} existing movies...");
                    _context.Movies.RemoveRange(existingMovies);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Seeding database with sample movies...");

                var sampleMovies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "The Shawshank Redemption",
                        Genre = "Drama",
                        Rating = 5,
                        PosterImage = "https://picsum.photos/seed/shawshank/400/600",
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Movie
                    {
                        Title = "The Godfather",
                        Genre = "Crime",
                        Rating = 5,
                        PosterImage = "https://picsum.photos/seed/godfather/400/600",
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        UpdatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new Movie
                    {
                        Title = "The Dark Knight",
                        Genre = "Action",
                        Rating = 5,
                        PosterImage = "https://picsum.photos/seed/darkknight/400/600",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Movie
                    {
                        Title = "Inception",
                        Genre = "Sci-Fi",
                        Rating = 4,
                        PosterImage = "https://picsum.photos/seed/inception/400/600",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Movie
                    {
                        Title = "Pulp Fiction",
                        Genre = "Crime",
                        Rating = 4,
                        PosterImage = "https://picsum.photos/seed/pulpfiction/400/600",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };

                await _context.Movies.AddRangeAsync(sampleMovies);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully seeded {sampleMovies.Count} movies to the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
