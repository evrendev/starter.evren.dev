using System.Text;
using System.Text.Encodings.Web;
using EvrenDev.Application.Common.Interfaces;
using OtpNet;

namespace EvrenDev.Infrastructure.Identity.Services;

public class TotpService : ITotpService
{
    private const string Issuer = "EvrenDev";
    private const int SecretKeyLength = 20;
    private readonly UrlEncoder _urlEncoder;

    public TotpService(UrlEncoder urlEncoder)
    {
        _urlEncoder = urlEncoder;
    }

    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(SecretKeyLength);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secretKey)
    {
        const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode(Issuer),
            _urlEncoder.Encode(email),
            secretKey);
    }

    public bool VerifyTotpCode(string secretKey, string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;

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

    private string FormatKey(string unformattedKey)
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
}
