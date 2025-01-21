using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface ICloudflareImageService
{
    /// <summary>
    /// Uploads an image to Cloudflare Images and returns the delivery URL
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <returns>The Cloudflare Images delivery URL for the uploaded image</returns>
    Task<string> UploadImageAsync(IFormFile file);
}
