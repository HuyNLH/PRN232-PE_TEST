using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PostManagementAPI.Configuration
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFileParameter = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType == typeof(IFormFile) || 
                          p.ParameterType == typeof(IEnumerable<IFormFile>));

            if (!hasFileParameter)
            {
                return;
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["name"] = new OpenApiSchema { Type = "string" },
                                ["description"] = new OpenApiSchema { Type = "string" },
                                ["imageUrl"] = new OpenApiSchema { Type = "string", Nullable = true },
                                ["imageFile"] = new OpenApiSchema { Type = "string", Format = "binary", Nullable = true }
                            },
                            Required = new HashSet<string> { "name", "description" }
                        }
                    }
                }
            };
        }
    }
}
