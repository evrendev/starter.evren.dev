using EvrenDev.Application.Identity.Tokens;
using EvrenDev.Application.Identity.TwoFactorAuthentication;

namespace EvrenDev.Application.Identity.Interfaces;

public interface ITotpService
{
    Task<TwoFactorAuthenticationDto> GenerateSetupAsync(TwoFactorSetupRequest request);
    Task<IEnumerable<string>?> EnableTwoFactorAuthenticationAsync(EnableTwoFactorAuthenticationRequest request);
    Task<bool> DisableTwoFactorAuthenticationAsync(DisableTwoFactorAuthenticationRequest request);

    Task<TokenResponse> VerifyTwoFactorAuthenticationAsync(VerifyTwoFactorAuthenticationRequest request,
        string ipAddress);
}
