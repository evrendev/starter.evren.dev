namespace EvrenDev.Application.Identity.Tokens;

public interface ITokenService : ITransientService
{
    Task<TokenResult> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken);

    Task<TokenResult> RefreshTokenAsync(string accessToken, string ipAddress);

    Task<TokenResult> GetTokenAfterTwoFactorAsync(string email, string ipAddress);
}
