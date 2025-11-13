using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PostManagementAPI.DTOs
{
    public class UpdateMovieWithFileDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        public string? Genre { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }

        [StringLength(500, ErrorMessage = "Poster URL cannot exceed 500 characters")]
        public string? PosterUrl { get; set; }

        public IFormFile? PosterFile { get; set; }
    }
}
