using System.Text;
using System.Text.Encodings.Web;
using EvrenDev.Application.Common.Interfaces;
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
    private readonly IStringLocalizer<TwoFactorAuthController> _localizer;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITotpService _totpService;

    public TwoFactorAuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IStringLocalizer<TwoFactorAuthController> localizer,
        ITokenService tokenService,
        IPermissionService permissionService,
        ITenantDbContext tenantDbContext,
        ITotpService totpService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _localizer = localizer;
        _tokenService = tokenService;
        _permissionService = permissionService;
        _tenantDbContext = tenantDbContext;
        _totpService = totpService;
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
        var secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(secretKey))
        {
            secretKey = _totpService.GenerateSecretKey();
            await _userManager.ResetAuthenticatorKeyAsync(user);
            secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var email = await _userManager.GetEmailAsync(user);
        if (string.IsNullOrEmpty(secretKey))
        {
            return BadRequest(_localizer["api.auth.2fa.setup-required"]);
        }
        var authenticatorUri = _totpService.GenerateQrCodeUri(email!, secretKey);

        return new TwoFactorAuthenticationDto
        {
            SharedKey = FormatKey(secretKey),
            QrCodeUri = authenticatorUri
        };
    }

    [HttpPost("enable")]
    public async Task<IActionResult> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(_localizer["api.auth.2fa.user-not-found"]);
        }

        var secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(secretKey))
        {
            return BadRequest(_localizer["api.auth.2fa.setup-required"]);
        }

        var verificationCode = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var isValid = _totpService.VerifyTotpCode(secretKey, verificationCode);

        if (!isValid)
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
    public async Task<ActionResult<AuthResponse>> VerifyTwoFactorCode(TwoFactorLoginRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(_localizer["api.auth.2fa.user-not-found"]);
        }

        var secretKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(secretKey))
        {
            return BadRequest(_localizer["api.auth.2fa.setup-required"]);
        }

        var isValid = _totpService.VerifyTotpCode(secretKey, request.Code);
        if (!isValid)
        {
            return BadRequest(_localizer["api.auth.2fa.invalid-code"]);
        }

        // Mark 2FA verification as complete for this session
        await _signInManager.SignInAsync(user, request.RememberMachine);

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
                Gender = user.Gender?.Code,
                Email = user.Email!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                FullName = user.FullName,
                Image = user.Image ?? "avatar.png",
                JobTitle = user.JobTitle ?? string.Empty,
                Language = user.Language?.Code,
                Permissions = permissions,
                TwoFactorEnabled = user.TwoFactorEnabled
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
}
