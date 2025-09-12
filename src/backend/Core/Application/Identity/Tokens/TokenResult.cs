namespace EvrenDev.Application.Identity.Tokens;

public record TokenResult(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiryTime, bool TwoFactorAuthRequired = false);
