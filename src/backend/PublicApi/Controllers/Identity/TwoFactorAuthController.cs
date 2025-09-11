using EvrenDev.Application.Identity.Interfaces;
using EvrenDev.Application.Identity.TwoFactorAuthentication;

namespace EvrenDev.PublicApi.Controllers.Identity;

[Route("api/2fa")]
public class TwoFactorAuthController(ITotpService totpService) : VersionNeutralApiController
{
    [HttpGet("setup")]
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
    [OpenApiOperation("Enable two-factor authentication.", "")]
    public async Task<ApiResponse<IEnumerable<string>?>> EnableTwoFactorAuthentication([FromBody] EnableTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.EnableTwoFactorAuthenticationAsync(request);
        return ApiResponse<IEnumerable<string>?>.Success(response);
    }

    [HttpPost("disable")]
    public async Task<ApiResponse<bool>> DisableTwoFactorAuthentication([FromBody] DisableTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.DisableTwoFactorAuthenticationAsync(request);
        return ApiResponse<bool>.Success(response);
    }
}
