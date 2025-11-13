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
    [Route("api/posts")]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PostsController> _logger;
        private readonly IImageUploadService _imageUploadService;

        public PostsController(
            AppDbContext context, 
            ILogger<PostsController> logger,
            IImageUploadService imageUploadService)
        {
            _context = context;
            _logger = logger;
            _imageUploadService = imageUploadService;
        }

        /// <summary>
        /// GET /api/posts - Get all posts with optional search, sort, and pagination
        /// </summary>
        /// <param name="q">Search query (partial, case-insensitive name search)</param>
        /// <param name="sort">Sort order: name_asc or name_desc</param>
        /// <param name="page">Page number (optional, default 1)</param>
        /// <param name="pageSize">Page size (optional, default 10)</param>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all posts",
            Description = "Retrieves all posts with optional search, sorting, and pagination support",
            OperationId = "GetPosts",
            Tags = new[] { "Posts" }
        )]
        [SwaggerResponse(200, "Posts retrieved successfully", typeof(IEnumerable<PostResponseDto>))]
        [SwaggerResponse(500, "Internal server error")]
        [ProducesResponseType(typeof(IEnumerable<PostResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetPosts(
            [FromQuery] string? q = null,
            [FromQuery] string? sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                IQueryable<Post> query = _context.Posts;

                // Search filter (partial, case-insensitive name search)
                if (!string.IsNullOrWhiteSpace(q))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(q.ToLower()));
                }

                // Sorting
                query = sort?.ToLower() switch
                {
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    "name_asc" => query.OrderBy(p => p.Name),
                    _ => query.OrderByDescending(p => p.CreatedAt) // Default: newest first
                };

                // Pagination (optional)
                if (page > 0 && pageSize > 0)
                {
                    query = query.Skip((page - 1) * pageSize).Take(pageSize);
                }

                var posts = await query.ToListAsync();

                var response = posts.Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Image = p.Image,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving posts");
                return StatusCode(500, new { message = "An error occurred while retrieving posts" });
            }
        }

        /// <summary>
        /// GET /api/posts/{id} - Get a single post by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostResponseDto>> GetPost(int id)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);

                if (post == null)
                {
                    return NotFound(new { message = $"Post with ID {id} not found" });
                }

                var response = new PostResponseDto
                {
                    Id = post.Id,
                    Name = post.Name,
                    Description = post.Description,
                    Image = post.Image,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving post {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the post" });
            }
        }

        /// <summary>
        /// POST /api/posts - Create a new post (supports both JSON and multipart/form-data)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json", "multipart/form-data")]
        public async Task<ActionResult<PostResponseDto>> CreatePost([FromForm] CreatePostWithFileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string? imageUrl = null;

                // If file is uploaded, validate and upload to Cloudinary
                if (dto.ImageFile != null)
                {
                    if (!_imageUploadService.IsValidImageFile(dto.ImageFile))
                    {
                        return BadRequest(new { message = "Invalid image file. File must be JPEG, PNG, GIF, or WebP and under 5MB." });
                    }

                    imageUrl = await _imageUploadService.UploadImageAsync(dto.ImageFile);
                }
                // Otherwise, use the URL if provided
                else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                {
                    imageUrl = dto.ImageUrl;
                }

                var post = new Post
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Image = imageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                var response = new PostResponseDto
                {
                    Id = post.Id,
                    Name = post.Name,
                    Description = post.Description,
                    Image = post.Image,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt
                };

                return CreatedAtAction(nameof(GetPost), new { id = post.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return StatusCode(500, new { message = "An error occurred while creating the post" });
            }
        }

        /// <summary>
        /// PUT /api/posts/{id} - Update an existing post (supports both JSON and multipart/form-data)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json", "multipart/form-data")]
        public async Task<ActionResult<PostResponseDto>> UpdatePost(int id, [FromForm] UpdatePostWithFileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var post = await _context.Posts.FindAsync(id);

                if (post == null)
                {
                    return NotFound(new { message = $"Post with ID {id} not found" });
                }

                // Handle image update
                if (dto.ImageFile != null)
                {
                    if (!_imageUploadService.IsValidImageFile(dto.ImageFile))
                    {
                        return BadRequest(new { message = "Invalid image file. File must be JPEG, PNG, GIF, or WebP and under 5MB." });
                    }

                    post.Image = await _imageUploadService.UploadImageAsync(dto.ImageFile);
                }
                else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                {
                    post.Image = dto.ImageUrl;
                }
                // If neither file nor URL provided, keep existing image

                post.Name = dto.Name;
                post.Description = dto.Description;
                post.UpdatedAt = DateTime.UtcNow;

                _context.Posts.Update(post);
                await _context.SaveChangesAsync();

                var response = new PostResponseDto
                {
                    Id = post.Id,
                    Name = post.Name,
                    Description = post.Description,
                    Image = post.Image,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the post" });
            }
        }

        /// <summary>
        /// DELETE /api/posts/{id} - Delete a post
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);

                if (post == null)
                {
                    return NotFound(new { message = $"Post with ID {id} not found" });
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the post" });
            }
        }
    }
}
