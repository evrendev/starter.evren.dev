using EvrenDev.Domain.Entities.Identity;

namespace EvrenDev.Application.Common.Interfaces;
public interface ITokenService
{
    string GenerateJwtToken(ApplicationUser user, List<string> roles);
    string GenerateRefreshToken();
    Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles, IList<string> permissions);
    Task<string> GenerateRefreshTokenAsync(string userId);
    Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
}
