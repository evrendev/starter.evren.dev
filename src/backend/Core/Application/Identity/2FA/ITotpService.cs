namespace EvrenDev.Application.Identity.Interfaces;

public interface ITotpService
{
    string GenerateSecretKey();
    string GenerateQrCodeUri(string email, string secretKey);
    bool VerifyTotpCode(string secretKey, string code);
    string GenerateTotpCode(string secretKey);
}
