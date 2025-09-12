using EvrenDev.Application.Identity.Interfaces;
using EvrenDev.Application.Identity.Tokens;
using EvrenDev.Application.Identity.TwoFactorAuthentication;

namespace EvrenDev.PublicApi.Controllers.Identity;

[Route("api/2fa")]
public class TwoFactorAuthController(ITotpService totpService) : VersionNeutralApiController
{
    [HttpGet("setup")]
    [Authorize]
    [OpenApiOperation("Get setup information for two-factor authentication.", "")]
    public async Task<ApiResponse<TwoFactorAuthenticationDto>> GetTwoFactorAuthenticationSetup([FromQuery] string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return ApiResponse<TwoFactorAuthenticationDto>.Failure("Id is required.");
        }

        var response = await totpService.GenerateSetupAsync(new TwoFactorSetupRequest { Id = id });
        return ApiResponse<TwoFactorAuthenticationDto>.Success(response);
    }

    [HttpPost("enable")]
    [Authorize]
    [OpenApiOperation("Enable two-factor authentication.", "")]
    public async Task<ApiResponse<IEnumerable<string>?>> EnableTwoFactorAuthentication([FromBody] EnableTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.EnableTwoFactorAuthenticationAsync(request);
        return ApiResponse<IEnumerable<string>?>.Success(response);
    }

    [HttpPost("disable")]
    [Authorize]
    [OpenApiOperation("Disable two-factor authentication.", "")]
    public async Task<ApiResponse<bool>> DisableTwoFactorAuthentication([FromBody] DisableTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.DisableTwoFactorAuthenticationAsync(request);
        return ApiResponse<bool>.Success(response);
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    [OpenApiOperation("Verify two-factor authentication code.", "")]
    public async Task<ApiResponse<TokenResponse>> VerifyTwoFactorAuthentication([FromBody] VerifyTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.VerifyTwoFactorAuthenticationAsync(request, GetIpAddress());

        // Add refresh token cookie to response
        Response.Cookies.Append("refresh_token", response.RefreshToken, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });

        return ApiResponse<TokenResponse>.Success(response);
    }

    private string GetIpAddress()
    {
        const string Na = "N/A";

        var headers = Request.Headers;
        if (headers.TryGetValue("X-Forwarded-For", out var forwardedForHeader))
        {
            return forwardedForHeader.ToString();
        }

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? Na;
    }
}
