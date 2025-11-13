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

                // Check if we already have movies
                if (await _context.Movies.AnyAsync())
                {
                    _logger.LogInformation("Database already contains movies. Skipping seed.");
                    return;
                }

                _logger.LogInformation("Seeding database with sample movies...");

                var sampleMovies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "The Shawshank Redemption",
                        Genre = "Drama",
                        Rating = 5,
                        PosterImage = "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Movie
                    {
                        Title = "The Godfather",
                        Genre = "Crime",
                        Rating = 5,
                        PosterImage = "https://image.tmdb.org/t/p/w500/3bhkrj58Vtu7enYsRolD1fZdja1.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        UpdatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new Movie
                    {
                        Title = "The Dark Knight",
                        Genre = "Action",
                        Rating = 5,
                        PosterImage = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Movie
                    {
                        Title = "Inception",
                        Genre = "Sci-Fi",
                        Rating = 4,
                        PosterImage = "https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Movie
                    {
                        Title = "Pulp Fiction",
                        Genre = "Crime",
                        Rating = 4,
                        PosterImage = "https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",
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
