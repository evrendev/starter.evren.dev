using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class CloudflareR2Service : ICloudflareR2Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly CloudflareSettings _settings;

    public CloudflareR2Service(IOptions<CloudflareSettings> settings)
    {
        _settings = settings.Value;

        var config = new AmazonS3Config
        {
            ServiceURL = _settings.R2.Endpoint,
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(
            _settings.R2.AccessKeyId,
            _settings.R2.SecretAccessKey,
            config
        );
    }

    public async Task<string> UploadFileAsync(IFormFile file, string bucketName, string? path = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        // Generate a unique file name
        var fileName = $"{Guid.NewGuid()}-{file.FileName}";

        // Combine path and filename if path is provided
        var key = string.IsNullOrEmpty(path)
            ? fileName
            : Path.Combine(path.TrimEnd('/'), fileName).Replace("\\", "/");

        // Prepare the upload request
        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = file.OpenReadStream(),
            ContentType = file.ContentType
        };

        // Upload the file
        await _s3Client.PutObjectAsync(putRequest);

        // Return the public URL
        return $"{_settings.R2.Endpoint}/{bucketName}/{key}";
    }

    public async Task DeleteFileAsync(string bucketName, string key)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }
}
