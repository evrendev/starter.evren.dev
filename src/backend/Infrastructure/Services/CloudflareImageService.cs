using System.Net.Http.Headers;
using Application.Common.Interfaces;
using EvrenDev.Shared.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class CloudflareImageService : ICloudflareImageService
{
    private readonly HttpClient _httpClient;
    private readonly CloudflareSettings _settings;

    public CloudflareImageService(IHttpClientFactory httpClientFactory, IOptions<CloudflareSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient("CloudflareImages");
        _settings = settings.Value;

        // Set Cloudflare API authentication headers
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Email", _settings.Email);
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Key", _settings.Apikey);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        if (!_settings.Images.AllowedTypes.Contains(file.ContentType))
            throw new ArgumentException("File type not allowed", nameof(file));

        if (file.Length > _settings.Images.MaxSizeInMb * 1024 * 1024)
            throw new ArgumentException($"File size exceeds maximum allowed size of {_settings.Images.MaxSizeInMb}MB", nameof(file));

        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

        content.Add(fileContent, "file", file.FileName);
        // Add responsive variants
        content.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(_settings.Images.ResponsiveWidths)), "variants");

        var response = await _httpClient.PostAsync($"https://api.cloudflare.com/client/v4/accounts/{_settings.AccountId}/images/v1", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to upload image: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = System.Text.Json.JsonSerializer.Deserialize<CloudflareImageResponse>(responseContent);

        if (result?.Result?.Id == null)
            throw new Exception("Failed to upload image");

        // Return the delivery URL with the default variant
        return _settings.Images.ImageDeliveryUrl.Replace("<image_id>", result.Result.Id).Replace("<variant_name>", "public");
    }
}

public class CloudflareImageResponse
{
    public bool Success { get; set; }
    public required CloudflareImageResult Result { get; set; }
}

public class CloudflareImageResult
{
    public required string Id { get; set; }
    public required string Filename { get; set; }
    public DateTime Uploaded { get; set; }
    public bool RequireSignedURLs { get; set; }
    public required Dictionary<string, string> Variants { get; set; }
}
