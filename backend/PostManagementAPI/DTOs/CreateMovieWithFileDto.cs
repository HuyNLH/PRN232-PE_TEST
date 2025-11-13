using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace PostManagementAPI.DTOs
{
    public class CreateMovieWithFileDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        [SwaggerSchema(Description = "The title of the movie")]
        public string Title { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        [SwaggerSchema(Description = "Movie genre (e.g., Action, Drama, Comedy)")]
        public string? Genre { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [SwaggerSchema(Description = "Movie rating from 1 to 5 stars")]
        public int? Rating { get; set; }

        [StringLength(500, ErrorMessage = "Poster URL cannot exceed 500 characters")]
        [SwaggerSchema(Description = "External poster image URL (alternative to file upload)")]
        public string? PosterUrl { get; set; }

        [SwaggerSchema(Description = "Poster image file to upload (JPEG, PNG, GIF, WebP, max 5MB)")]
        public IFormFile? PosterFile { get; set; }
    }
}
