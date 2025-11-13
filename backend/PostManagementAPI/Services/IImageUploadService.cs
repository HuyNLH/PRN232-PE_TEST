using Microsoft.AspNetCore.Http;

namespace PostManagementAPI.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file);
        bool IsValidImageFile(IFormFile file);
    }
}
