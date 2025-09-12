using System.Text;
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
    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(" ");
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string secretKey)
    {
        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode(Issuer),
            _urlEncoder.Encode(email),
            secretKey);
    }

    private static bool VerifyTotpCode(string secretKey, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        code = code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var keyBytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(keyBytes);

        return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
    }

    public async Task<TwoFactorAuthenticationDto> GenerateSetupAsync(TwoFactorSetupRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id) ?? throw new Exception("User not found.");

        await _userManager.ResetAuthenticatorKeyAsync(user);

        var secretKey = await _userManager.GetAuthenticatorKeyAsync(user);

        if (secretKey is null)
        {
            throw new Exception("Could not generate authenticator key.");
        }

        var qrCodeUri = GenerateQrCodeUri(user.Email, secretKey);

        return new TwoFactorAuthenticationDto
        {
            SharedKey = FormatKey(secretKey),
            QrCodeUri = qrCodeUri
        };
    }

    public async Task<IEnumerable<string>?> EnableTwoFactorAuthenticationAsync(EnableTwoFactorAuthenticationRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id) ?? throw new Exception("User not found.");

        var unverifiedSecretKey = await _userManager.GetAuthenticatorKeyAsync(user);

        if (unverifiedSecretKey is null)
        {
            throw new Exception("No unverified secret key found for the user.");
        }

        bool isCodeValid = VerifyTotpCode(unverifiedSecretKey, request.Code);

        if (!isCodeValid)
        {
            throw new Exception("Invalid two-factor authentication code.");
        }

        var setResult = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!setResult.Succeeded)
        {
            throw new Exception("Failed to enable two-factor authentication.");
        }

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

    public async Task<bool> VerifyTwoFactorAuthenticationAsync(VerifyTwoFactorAuthenticationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.TwoFactorEnabled)
            throw new Exception("Invalid user or two-factor authentication is not enabled.");

        var secretKey = await _userManager.GetAuthenticationTokenAsync(user, "EvrenDev", "AuthenticatorKey");

        if (secretKey is null)
            throw new Exception("No authenticator key found for the user.");

        return VerifyTotpCode(secretKey, request.Code);
    }
}
