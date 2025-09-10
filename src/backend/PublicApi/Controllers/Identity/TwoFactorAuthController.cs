using EvrenDev.Application.Identity.Interfaces;
using EvrenDev.Application.Identity.TwoFactorAuthentication;

namespace EvrenDev.PublicApi.Controllers.Identity;

public class TwoFactorAuthController(ITotpService totpService) : VersionNeutralApiController
{
    [HttpGet("setup")]
    [OpenApiOperation("Get setup information for two-factor authentication.", "")]
    public async Task<ActionResult<TwoFactorAuthenticationDto>> GetTwoFactorAuthenticationSetup(TwoFactorSetupRequest request)
    {
        var response = await totpService.GenerateSetupAsync(request);
        return Ok(response);
    }

    [HttpPost("enable")]
    [OpenApiOperation("Enable two-factor authentication.", "")]
    public async Task<IActionResult> EnableTwoFactorAuthentication([FromBody] EnableTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.EnableTwoFactorAuthenticationAsync(request);
        return Ok(response);
    }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableTwoFactorAuthentication(DisableTwoFactorAuthenticationRequest request)
    {
        var response = await totpService.DisableTwoFactorAuthenticationAsync(request);
        return Ok(response);
    }
}
