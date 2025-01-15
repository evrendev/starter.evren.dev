using System.Text;
using System.Text.Encodings.Web;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/2fa")]
public class TwoFactorAuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UrlEncoder _urlEncoder;
    private readonly IStringLocalizer<TwoFactorAuthController> _localizer;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;
    private readonly ITenantDbContext _tenantDbContext;

    public TwoFactorAuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        UrlEncoder urlEncoder,
        IStringLocalizer<TwoFactorAuthController> localizer,
        ITokenService tokenService,
        IPermissionService permissionService,
        ITenantDbContext tenantDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _urlEncoder = urlEncoder;
        _localizer = localizer;
        _tokenService = tokenService;
        _permissionService = permissionService;
        _tenantDbContext = tenantDbContext;
    }

    [HttpGet("setup")]
    public async Task<ActionResult<TwoFactorAuthenticationDto>> GetTwoFactorAuthenticationSetup()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(_localizer["api.auth.2fa.user-not-found"]);
        }

        // Generate the key and QR code URL
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var email = await _userManager.GetEmailAsync(user);
        var authenticatorUri = GenerateQrCodeUri(email!, unformattedKey!);

        return new TwoFactorAuthenticationDto
        {
            SharedKey = FormatKey(unformattedKey!),
            QrCodeUri = authenticatorUri
        };
    }

    [HttpPost("enable")]
    public async Task<IActionResult> EnableTwoFactorAuthentication([FromBody] EnableTwoFactorAuthenticationRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(_localizer["api.auth.2fa.user-not-found"]);
        }

        var verificationCode = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            return BadRequest(_localizer["api.auth.2fa.invalid-code"]);
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return Ok();
    }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableTwoFactorAuthentication()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(_localizer["api.auth.2fa.user-not-found"]);
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            return BadRequest(_localizer["api.auth.2fa.disable-failed"]);
        }

        return Ok();
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> VerifyTwoFactorCode([FromBody] TwoFactorLoginRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(_localizer["api.auth.2fa.user-not-found"]);
        }

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            request.Code, request.RememberMachine, false);

        if (!result.Succeeded)
        {
            return BadRequest(_localizer["api.auth.2fa.invalid-code"]);
        }

        var tenant = await _tenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == user.TenantId);
        if (tenant == null || !tenant.IsActive || tenant.Deleted)
        {
            return BadRequest(_localizer["api.auth.login.invalid-tenant"]);
        }

        var permissions = await _permissionService.GetUserPermissions(user.Id);
        var token = await _tokenService.GenerateJwtTokenAsync(user, permissions);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

        var response = new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                Gender = user.Gender,
                Email = user.Email!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                FullName = user.FullName,
                Image = user.Image ?? "avatar.png",
                JobTitle = user.JobTitle ?? string.Empty,
                Language = user.Language,
                Permissions = permissions
            }
        };

        return Ok(response);
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

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode("EvrenDev"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }
}
