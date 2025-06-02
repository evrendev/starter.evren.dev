namespace EvrenDev.Application.Identity.Tokens;

public record TokenResponse(string Token, DateTime RefreshTokenExpiryTime);
