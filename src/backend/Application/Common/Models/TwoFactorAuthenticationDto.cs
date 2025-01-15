using System.Text.Json.Serialization;

namespace EvrenDev.Application.Common.Models;

public class TwoFactorAuthenticationDto
{
    public string SharedKey { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
}

public class EnableTwoFactorAuthenticationRequest
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

public class TwoFactorLoginRequest
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("rememberMachine")]
    public bool RememberMachine { get; set; }
}
