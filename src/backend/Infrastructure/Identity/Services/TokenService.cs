using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EvrenDev.Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenService> _logger;
        private readonly IPermissionService _permissionService;

        public TokenService(
            IConfiguration configuration,
            IDistributedCache cache,
            ILogger<TokenService> logger,
            IPermissionService permissionService)
        {
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
            _permissionService = permissionService;
        }

        public string GenerateJwtToken(ApplicationUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("fullname", user.FullName ?? string.Empty),
                new("tenant", user.TenantId ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles, IList<string> permissions)
        {
            _logger.LogInformation("Generating JWT token for user {UserId} with roles: {Roles}",
                user.Id, string.Join(", ", roles));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("tenant", user.TenantId ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Get user permissions
            var userPermissions = await _permissionService.GetUserPermissions(user.Id);
            _logger.LogInformation("User {UserId} has permissions: {Permissions}",
                user.Id, string.Join(", ", userPermissions));

            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            _logger.LogInformation("Generated JWT token with claims: {Claims}",
                string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            var refreshToken = GenerateRefreshToken();
            await _cache.SetStringAsync(
                $"RefreshToken_{userId}",
                refreshToken,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
        {
            var storedToken = await _cache.GetStringAsync($"RefreshToken_{userId}");
            return storedToken == refreshToken;
        }
    }
}
