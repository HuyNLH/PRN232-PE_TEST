using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PostManagementAPI.Configuration;

namespace PostManagementAPI.Services
{
    public class CloudinaryImageUploadService : IImageUploadService
    {
        private readonly CloudinarySettings _settings;
        private readonly Cloudinary? _cloudinary;
        private readonly ILogger<CloudinaryImageUploadService> _logger;

        // Maximum file size: 5MB
        private const long MaxFileSize = 5 * 1024 * 1024;

        // Allowed image MIME types
        private readonly string[] _allowedMimeTypes = new[]
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        public CloudinaryImageUploadService(
            IOptions<CloudinarySettings> settings,
            ILogger<CloudinaryImageUploadService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            // Only initialize Cloudinary if settings are provided and enabled
            if (_settings.Enabled &&
                !string.IsNullOrEmpty(_settings.CloudName) &&
                !string.IsNullOrEmpty(_settings.ApiKey) &&
                !string.IsNullOrEmpty(_settings.ApiSecret))
            {
                var account = new Account(
                    _settings.CloudName,
                    _settings.ApiKey,
                    _settings.ApiSecret
                );
                _cloudinary = new Cloudinary(account);
                _logger.LogInformation("Cloudinary service initialized successfully");
            }
            else
            {
                _logger.LogWarning("Cloudinary is not configured. Image uploads will return placeholder URLs.");
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Check file size
            if (file.Length > MaxFileSize)
            {
                _logger.LogWarning("File size {Size} exceeds maximum allowed size {MaxSize}", 
                    file.Length, MaxFileSize);
                return false;
            }

            // Check MIME type
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                _logger.LogWarning("Invalid file type: {ContentType}", file.ContentType);
                return false;
            }

            return true;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (!IsValidImageFile(file))
            {
                throw new ArgumentException("Invalid image file. Must be JPEG, PNG, GIF, or WebP and less than 5MB.");
            }

            // If Cloudinary is not configured, return a placeholder URL
            if (_cloudinary == null)
            {
                _logger.LogWarning("Cloudinary not configured. Returning placeholder URL.");
                return $"https://via.placeholder.com/800x600?text={Uri.EscapeDataString(file.FileName)}";
            }

            try
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "post-management",
                    Transformation = new Transformation()
                        .Width(1200)
                        .Height(800)
                        .Crop("limit")
                        .Quality("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Image uploaded successfully to Cloudinary: {Url}", 
                        uploadResult.SecureUrl.ToString());
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    _logger.LogError("Cloudinary upload failed with status {Status}: {Error}", 
                        uploadResult.StatusCode, uploadResult.Error?.Message);
                    throw new Exception($"Image upload failed: {uploadResult.Error?.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                throw;
            }
        }
    }
}
