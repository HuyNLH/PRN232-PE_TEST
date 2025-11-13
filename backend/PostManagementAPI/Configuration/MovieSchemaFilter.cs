using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using PostManagementAPI.DTOs;

namespace PostManagementAPI.Configuration
{
    /// <summary>
    /// Adds example data to Movie-related schemas in Swagger documentation
    /// </summary>
    public class MovieSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Add examples for CreateMovieDto
            if (context.Type == typeof(CreateMovieDto))
            {
                schema.Example = new OpenApiObject
                {
                    ["title"] = new OpenApiString("Inception"),
                    ["genre"] = new OpenApiString("Sci-Fi"),
                    ["rating"] = new OpenApiInteger(5),
                    ["posterUrl"] = new OpenApiString("https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg")
                };
            }
            
            // Add examples for UpdateMovieDto
            if (context.Type == typeof(UpdateMovieDto))
            {
                schema.Example = new OpenApiObject
                {
                    ["title"] = new OpenApiString("The Matrix Reloaded"),
                    ["genre"] = new OpenApiString("Action"),
                    ["rating"] = new OpenApiInteger(4),
                    ["posterUrl"] = new OpenApiString("https://image.tmdb.org/t/p/w500/example.jpg")
                };
            }
            
            // Add examples for MovieResponseDto
            if (context.Type == typeof(MovieResponseDto))
            {
                schema.Example = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["title"] = new OpenApiString("The Shawshank Redemption"),
                    ["genre"] = new OpenApiString("Drama"),
                    ["rating"] = new OpenApiInteger(5),
                    ["posterImage"] = new OpenApiString("https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg"),
                    ["createdAt"] = new OpenApiString("2025-01-03T10:30:00Z"),
                    ["updatedAt"] = new OpenApiString("2025-01-03T10:30:00Z")
                };
            }
        }
    }
}
