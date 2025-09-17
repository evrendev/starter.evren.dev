using EvrenDev.Application.Identity.Tokens;

namespace EvrenDev.PublicApi.Controllers.Identity;

public sealed class AuthController(ITokenService tokenService) : VersionNeutralApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public async Task<ApiResponse<TokenResponse>?> GetTokenAsync(
        TokenRequest request,
        CancellationToken cancellationToken)
    {
        var tokenResult = await tokenService.GetTokenAsync(request, GetIpAddress(), cancellationToken);
        AddRefreshTokenCookie(tokenResult.RefreshToken);

        if (tokenResult.TwoFactorAuthRequired)
            return ApiResponse<TokenResponse>.Success(new TokenResponse(
                string.Empty,
                string.Empty,
                DateTime.MinValue,
                true
            ));

        var data = new TokenResponse(tokenResult.AccessToken, tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiryTime);

        return ApiResponse<TokenResponse>.Success(data);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public ActionResult<TokenResponse> RemoveTokenAsync()
    {
        Response.Cookies.Delete("refresh_token", CreateCookeOptions());
        return Ok();
    }

    [HttpGet("refresh-token")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using a refresh token.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Search))]
    public async Task<ApiResponse<TokenResponse>> RefreshAsync()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var accessToken))
            throw new UnauthorizedException("Refresh token is missing.");

        try
        {
            var tokenResult = await tokenService.RefreshTokenAsync(accessToken, GetIpAddress());
            var data = new TokenResponse(tokenResult.AccessToken, tokenResult.RefreshToken,
                tokenResult.RefreshTokenExpiryTime);
            AddRefreshTokenCookie(tokenResult.RefreshToken);

            return ApiResponse<TokenResponse>.Success(data);
        }
        catch (UnauthorizedException)
        {
            Response.Cookies.Delete("refresh_token", CreateCookeOptions());
            throw;
        }
    }

    private void AddRefreshTokenCookie(string refreshToken)
    {
        // Apply RefreshToken to secure cookie
        Response.Cookies.Append("refresh_token", refreshToken, CreateCookeOptions());
    }

    private static CookieOptions CreateCookeOptions()
    {
        return new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true };
    }

    private string GetIpAddress()
    {
        const string Na = "N/A";

        var headers = Request.Headers;
        if (headers.TryGetValue("X-Forwarded-For", out var forwardedForHeader)) return forwardedForHeader.ToString();

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? Na;
    }
}
