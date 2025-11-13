using Swashbuckle.AspNetCore.Annotations;

namespace PostManagementAPI.DTOs
{
    public class PostResponseDto
    {
        [SwaggerSchema(Description = "Unique identifier for the post")]
        public int Id { get; set; }
        
        [SwaggerSchema(Description = "The name/title of the post")]
        public string Name { get; set; } = string.Empty;
        
        [SwaggerSchema(Description = "Detailed description of the post")]
        public string Description { get; set; } = string.Empty;
        
        [SwaggerSchema(Description = "Image URL (Cloudinary URL or external URL)")]
        public string? Image { get; set; }
        
        [SwaggerSchema(Description = "Timestamp when the post was created")]
        public DateTime CreatedAt { get; set; }
        
        [SwaggerSchema(Description = "Timestamp when the post was last updated")]
        public DateTime UpdatedAt { get; set; }
    }
}
