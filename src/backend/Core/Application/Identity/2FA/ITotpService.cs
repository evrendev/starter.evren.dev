using EvrenDev.Application.Identity.TwoFactorAuthentication;

namespace EvrenDev.Application.Identity.Interfaces;

public interface ITotpService
{
    Task<TwoFactorAuthenticationDto> GenerateSetupAsync(TwoFactorSetupRequest request);
    Task<IEnumerable<string>?> EnableTwoFactorAuthenticationAsync(EnableTwoFactorAuthenticationRequest request);
    Task<bool> DisableTwoFactorAuthenticationAsync(DisableTwoFactorAuthenticationRequest request);
    string GenerateSecretKey();
    string GenerateQrCodeUri(string email, string secretKey);
    bool VerifyTotpCode(string secretKey, string code);
    string GenerateTotpCode(string secretKey);
}
