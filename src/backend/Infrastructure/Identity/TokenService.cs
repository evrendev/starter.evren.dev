using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Identity.Tokens;
using EvrenDev.Domain.Identity;
using EvrenDev.Domain.Multitenancy;
using EvrenDev.Infrastructure.Auth;
using EvrenDev.Infrastructure.Auth.Jwt;
using EvrenDev.Infrastructure.Common.ReCaptcha;
using EvrenDev.Shared.Authorization;
using EvrenDev.Shared.Multitenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EvrenDev.Infrastructure.Identity;

internal class TokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        IStringLocalizer<TokenService> localizer,
        TenantInfo? currentTenant,
        IOptions<SecuritySettings> securitySettings,
        ReCaptchaClient reCaptchaClient)
    : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task<TokenResult> GetTokenAsync(TokenRequest request, string ipAddress,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currentTenant?.Id))
            throw new UnauthorizedException(localizer["multitenancy.tenant.invalid"]);

        if (_securitySettings.RequireReCaptcha && !await reCaptchaClient.IsValid(request.Response))
            throw new UnauthorizedException(localizer["identity.auth.invalidcaptcha"]);

        var user = await userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        if (user is null) throw new UnauthorizedException(localizer["identity.auth.failed"]);

        if (!user.IsActive) throw new UnauthorizedException(localizer["identity.users.notactive"]);

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
            throw new UnauthorizedException(localizer["identity.users.emailnotconfirmed"]);

        if (currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!currentTenant.IsActive) throw new UnauthorizedException(localizer["multitenancy.tenant.inactive"]);

            if (DateTime.UtcNow > currentTenant.ValidUpto)
                throw new UnauthorizedException(localizer["multitenancy.tenant.expired"]);
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException(localizer["identity.auth.invalidcredentials"]);

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    public async Task<TokenResult> RefreshTokenAsync(string accessToken, string ipAddress)
    {
        var user = userManager.Users.FirstOrDefault(u => u.RefreshToken == accessToken);
        if (user is null) throw new UnauthorizedException(localizer["identity.auth.failed"]);

        if (user.RefreshToken != accessToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedException(localizer["identity.auth.invalidrefreshtoken"]);

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    public async Task<TokenResult> GetTokenAfterTwoFactorAsync(string email, string ipAddress)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new UnauthorizedException(localizer["identity.auth.failed"]);

        if (!user.IsActive) throw new UnauthorizedException(localizer["identity.users.notactive"]);

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
            throw new UnauthorizedException(localizer["identity.users.emailnotconfirmed"]);

        if (currentTenant?.Id != MultitenancyConstants.Root.Id)
        {
            if (!currentTenant!.IsActive) throw new UnauthorizedException(localizer["multitenancy.tenant.inactive"]);

            if (DateTime.UtcNow > currentTenant.ValidUpto)
                throw new UnauthorizedException(localizer["multitenancy.tenant.expired"]);
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    private async Task<TokenResult> GenerateTokensAndUpdateUser(ApplicationUser user, string ipAddress)
    {
        var token = GenerateJwt(user, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await userManager.UpdateAsync(user);

        return new TokenResult(token, user.RefreshToken, user.RefreshTokenExpiryTime, user.TwoFactorEnabled);
    }

    private string GenerateJwt(ApplicationUser user, string ipAddress)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));
    }

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress)
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ApiClaims.Fullname, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ApiClaims.IpAddress, ipAddress),
            new(ApiClaims.Tenant, currentTenant!.Id),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
            throw new InvalidOperationException("No Key defined in JwtSettings config.");

        var secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}
