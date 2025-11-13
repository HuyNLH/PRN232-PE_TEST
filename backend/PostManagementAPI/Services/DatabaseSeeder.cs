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

                // Check if we already have posts
                if (await _context.Posts.AnyAsync())
                {
                    _logger.LogInformation("Database already contains posts. Skipping seed.");
                    return;
                }

                _logger.LogInformation("Seeding database with sample posts...");

                var samplePosts = new List<Post>
                {
                    new Post
                    {
                        Name = "Beautiful Mountain Landscape",
                        Description = "A breathtaking view of snow-capped mountains during golden hour. The peaks are illuminated by the setting sun, creating a stunning contrast against the deep blue sky.",
                        Image = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4",
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Post
                    {
                        Name = "Ocean Waves at Sunset",
                        Description = "Powerful ocean waves crashing against the shore as the sun sets on the horizon. The golden light reflects beautifully on the water surface, creating a mesmerizing scene.",
                        Image = "https://images.unsplash.com/photo-1505142468610-359e7d316be0",
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        UpdatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new Post
                    {
                        Name = "Autumn Forest Path",
                        Description = "A winding path through a dense forest in autumn. The trees are ablaze with vibrant reds, oranges, and yellows, creating a magical woodland atmosphere perfect for a peaceful walk.",
                        Image = "https://images.unsplash.com/photo-1511497584788-876760111969",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Post
                    {
                        Name = "City Skyline at Night",
                        Description = "A modern city skyline illuminated at night with thousands of lights from buildings and streets. The urban landscape creates a stunning display of human achievement and architectural beauty.",
                        Image = "https://images.unsplash.com/photo-1514565131-fce0801e5785",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Post
                    {
                        Name = "Desert Sand Dunes",
                        Description = "Majestic sand dunes stretching as far as the eye can see in the desert. The smooth curves and ripples in the sand create beautiful patterns, highlighted by the warm desert sunlight.",
                        Image = "https://images.unsplash.com/photo-1509316785289-025f5b846b35",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };

                await _context.Posts.AddRangeAsync(samplePosts);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully seeded {samplePosts.Count} posts to the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
