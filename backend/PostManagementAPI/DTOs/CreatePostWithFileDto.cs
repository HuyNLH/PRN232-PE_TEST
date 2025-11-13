using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace PostManagementAPI.DTOs
{
    public class CreatePostWithFileDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        [SwaggerSchema(Description = "The name/title of the post")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        [SwaggerSchema(Description = "Detailed description of the post")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [SwaggerSchema(Description = "External image URL (alternative to file upload)")]
        public string? ImageUrl { get; set; }

        [SwaggerSchema(Description = "Image file to upload (JPEG, PNG, GIF, WebP, max 5MB)")]
        public IFormFile? ImageFile { get; set; }
    }
}
