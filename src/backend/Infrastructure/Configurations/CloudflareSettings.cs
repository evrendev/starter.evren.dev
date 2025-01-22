namespace Infrastructure.Configurations;

public class CloudflareSettings
{
    public required string AccountId { get; set; }
    public required string Apikey { get; set; }
    public required string Email { get; set; }
    public required ImageSettings Images { get; set; }
    public required R2Settings R2 { get; set; }

    public class ImageSettings
    {
        public int MaxSizeInMb { get; set; } = 10;
        public string[] AllowedTypes { get; set; } = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        public string[] ResponsiveWidths { get; set; } = { "100", "300", "500", "700", "900", "1100" };
        public required string AccountHash { get; set; }
        public required string ImageDeliveryUrl { get; set; }
    }

    public class R2Settings
    {
        public required string Endpoint { get; set; }
        public required string Token { get; set; }
        public required string AccessKeyId { get; set; }
        public required string SecretAccessKey { get; set; }
    }
}
