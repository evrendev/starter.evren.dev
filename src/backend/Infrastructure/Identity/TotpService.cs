using System.Text.Encodings.Web;
using EvrenDev.Application.Identity.Interfaces;
using EvrenDev.Application.Identity.TwoFactorAuthentication;
using EvrenDev.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using OtpNet;

namespace EvrenDev.Infrastructure.Identity;

public class TotpService : ITotpService
{
    private const string Issuer = "EvrenDev";
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UrlEncoder _urlEncoder;

    public TotpService(
        UserManager<ApplicationUser> userManager,
        UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _urlEncoder = urlEncoder;
    }

    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secretKey)
    {
        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode(Issuer),
            _urlEncoder.Encode(email),
            secretKey);
    }

    public bool VerifyTotpCode(string secretKey, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        code = code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var keyBytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(keyBytes);

        return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
    }

    public string GenerateTotpCode(string secretKey)
    {
        var keyBytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(keyBytes);
        return totp.ComputeTotp();
    }

    public async Task<TwoFactorAuthenticationDto> GenerateSetupAsync(TwoFactorSetupRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id) ?? throw new Exception("User not found.");

        var secretKey = GenerateSecretKey();

        await _userManager.SetAuthenticationTokenAsync(user, "Default", "AuthenticatorKey", secretKey);

        var qrCodeUri = GenerateQrCodeUri(user.Email, secretKey);

        return new TwoFactorAuthenticationDto
        {
            SharedKey = secretKey,
            QrCodeUri = qrCodeUri
        };
    }

    public async Task<IEnumerable<string>?> EnableTwoFactorAuthenticationAsync(EnableTwoFactorAuthenticationRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id) ?? throw new Exception("User not found.");

        var unverifiedSecretKey = await _userManager.GetAuthenticationTokenAsync(user, "Default", "AuthenticatorKey");

        if (unverifiedSecretKey is null)
        {
            throw new Exception("No unverified secret key found for the user.");
        }

        bool isCodeValid = VerifyTotpCode(unverifiedSecretKey, request.Code);

        if (!isCodeValid)
        {
            throw new Exception("Invalid two-factor authentication code.");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return recoveryCodes;
    }

    public async Task<bool> DisableTwoFactorAuthenticationAsync(DisableTwoFactorAuthenticationRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id) ?? throw new Exception("User not found.");

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            return false;
        }

        return true;
    }
}
