using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface ICloudflareR2Service
{
    /// <summary>
    /// Uploads a file to Cloudflare R2 storage and returns the file URL
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="bucketName">The name of the bucket to upload to</param>
    /// <param name="path">Optional path within the bucket (e.g., "documents/2024/")</param>
    /// <returns>The URL of the uploaded file</returns>
    Task<string> UploadFileAsync(IFormFile file, string bucketName, string? path = null);

    /// <summary>
    /// Deletes a file from Cloudflare R2 storage
    /// </summary>
    /// <param name="bucketName">The name of the bucket</param>
    /// <param name="key">The key (path) of the file to delete</param>
    Task DeleteFileAsync(string bucketName, string key);
}
