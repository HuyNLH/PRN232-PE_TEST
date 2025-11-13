using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostManagementAPI.Data;
using PostManagementAPI.DTOs;
using PostManagementAPI.Models;
using PostManagementAPI.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PostManagementAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    [Produces("application/json")]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MoviesController> _logger;
        private readonly IImageUploadService _imageUploadService;

        public MoviesController(
            AppDbContext context, 
            ILogger<MoviesController> logger,
            IImageUploadService imageUploadService)
        {
            _context = context;
            _logger = logger;
            _imageUploadService = imageUploadService;
        }

        /// <summary>
        /// GET /api/movies - Get all movies with optional search, filter by genre, sort, and pagination
        /// </summary>
        /// <param name="q">Search query (partial, case-insensitive title search)</param>
        /// <param name="genre">Filter by genre</param>
        /// <param name="sort">Sort order: title_asc, title_desc, rating_asc, or rating_desc</param>
        /// <param name="page">Page number (optional, default 1)</param>
        /// <param name="pageSize">Page size (optional, default 10)</param>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all movies",
            Description = "Retrieves all movies with optional search by title, filter by genre, sorting, and pagination support",
            OperationId = "GetMovies",
            Tags = new[] { "Movies" }
        )]
        [SwaggerResponse(200, "Movies retrieved successfully", typeof(IEnumerable<MovieResponseDto>))]
        [SwaggerResponse(500, "Internal server error")]
        [ProducesResponseType(typeof(IEnumerable<MovieResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MovieResponseDto>>> GetMovies(
            [FromQuery] string? q = null,
            [FromQuery] string? genre = null,
            [FromQuery] string? sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                IQueryable<Movie> query = _context.Movies;

                // Search filter (partial, case-insensitive title search)
                if (!string.IsNullOrWhiteSpace(q))
                {
                    query = query.Where(m => m.Title.ToLower().Contains(q.ToLower()));
                }

                // Genre filter
                if (!string.IsNullOrWhiteSpace(genre))
                {
                    query = query.Where(m => m.Genre != null && m.Genre.ToLower() == genre.ToLower());
                }

                // Sorting
                query = sort?.ToLower() switch
                {
                    "title_desc" => query.OrderByDescending(m => m.Title),
                    "title_asc" => query.OrderBy(m => m.Title),
                    "rating_asc" => query.OrderBy(m => m.Rating ?? 0),
                    "rating_desc" => query.OrderByDescending(m => m.Rating ?? 0),
                    _ => query.OrderByDescending(m => m.CreatedAt) // Default: newest first
                };

                // Pagination (optional)
                if (page > 0 && pageSize > 0)
                {
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);
                }

                var movies = await query.ToListAsync();

                var response = movies.Select(m => new MovieResponseDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Genre = m.Genre,
                    Rating = m.Rating,
                    PosterImage = m.PosterImage,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movies");
                return StatusCode(500, new { message = "An error occurred while retrieving movies" });
            }
        }

        /// <summary>
        /// GET /api/movies/{id} - Get a single movie by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieResponseDto>> GetMovie(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return NotFound(new { message = $"Movie with ID {id} not found" });
                }

                var response = new MovieResponseDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Genre = movie.Genre,
                    Rating = movie.Rating,
                    PosterImage = movie.PosterImage,
                    CreatedAt = movie.CreatedAt,
                    UpdatedAt = movie.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the movie" });
            }
        }

        /// <summary>
        /// POST /api/movies - Create a new Movie (supports both JSON and multipart/form-data)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json", "multipart/form-data")]
        public async Task<ActionResult<MovieResponseDto>> CreateMovie([FromForm] CreateMovieWithFileDto dto)
        {
            _logger.LogInformation("=== CreateMovie called ===");
            _logger.LogInformation($"Title: {dto.Title}, Genre: {dto.Genre}, Rating: {dto.Rating}");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var error in ModelState)
                {
                    _logger.LogWarning($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return BadRequest(ModelState);
            }

            try
            {
                string? imageUrl = null;

                // Debug logging
                _logger.LogInformation($"CreateMovie - PosterFile: {dto.PosterFile?.FileName ?? "NULL"}, PosterUrl: {dto.PosterUrl ?? "NULL"}");

                // If file is uploaded, validate and upload to Cloudinary
                if (dto.PosterFile != null)
                {
                    _logger.LogInformation($"Uploading file: {dto.PosterFile.FileName}");
                    if (!_imageUploadService.IsValidImageFile(dto.PosterFile))
                    {
                        return BadRequest(new { message = "Invalid image file. File must be JPEG, PNG, GIF, or WebP and under 5MB." });
                    }

                    imageUrl = await _imageUploadService.UploadImageAsync(dto.PosterFile);
                    _logger.LogInformation($"File uploaded successfully: {imageUrl}");
                }
                // Otherwise, use the URL if provided
                else if (!string.IsNullOrWhiteSpace(dto.PosterUrl))
                {
                    _logger.LogInformation($"Using URL: {dto.PosterUrl}");
                    imageUrl = dto.PosterUrl;
                }

                var movie = new Movie
                {
                    Title = dto.Title,
                    Genre = dto.Genre,
                    Rating = dto.Rating,
                    PosterImage = imageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                var response = new MovieResponseDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Genre = movie.Genre,
                    Rating = movie.Rating,
                    PosterImage = movie.PosterImage,
                    CreatedAt = movie.CreatedAt,
                    UpdatedAt = movie.UpdatedAt
                };

                return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie");
                return StatusCode(500, new { message = "An error occurred while creating the movie" });
            }
        }

        /// <summary>
        /// PUT /api/movies/{id} - Update an existing post (supports both JSON and multipart/form-data)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json", "multipart/form-data")]
        public async Task<ActionResult<MovieResponseDto>> UpdateMovie(int id, [FromForm] UpdateMovieWithFileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return NotFound(new { message = $"Movie with ID {id} not found" });
                }

                // Handle image update
                if (dto.PosterFile != null)
                {
                    if (!_imageUploadService.IsValidImageFile(dto.PosterFile))
                    {
                        return BadRequest(new { message = "Invalid image file. File must be JPEG, PNG, GIF, or WebP and under 5MB." });
                    }

                    movie.PosterImage = await _imageUploadService.UploadImageAsync(dto.PosterFile);
                }
                else if (!string.IsNullOrWhiteSpace(dto.PosterUrl))
                {
                    movie.PosterImage = dto.PosterUrl;
                }
                // If neither file nor URL provided, keep existing image

                movie.Title = dto.Title;
                movie.Genre = dto.Genre;
                movie.Rating = dto.Rating;
                movie.UpdatedAt = DateTime.UtcNow;

                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                var response = new MovieResponseDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Genre = movie.Genre,
                    Rating = movie.Rating,
                    PosterImage = movie.PosterImage,
                    CreatedAt = movie.CreatedAt,
                    UpdatedAt = movie.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the movie" });
            }
        }

        /// <summary>
        /// DELETE /api/movies/{id} - Delete a post
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return NotFound(new { message = $"Movie with ID {id} not found" });
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the movie" });
            }
        }
    }
}
