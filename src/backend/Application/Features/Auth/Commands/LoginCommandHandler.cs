using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;
    private readonly IStringLocalizer<LoginCommandHandler> _localizer;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITenantDbContext tenantDbContext,
        ITokenService tokenService,
        IPermissionService permissionService,
        IStringLocalizer<LoginCommandHandler> localizer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tenantDbContext = tenantDbContext;
        _tokenService = tokenService;
        _permissionService = permissionService;
        _localizer = localizer;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result<AuthResponse>.Failure(_localizer["api.auth.login.not-found"]);

        if (user.Deleted)
            return Result<AuthResponse>.Failure(_localizer["api.auth.login.deleted"]);

        var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);

        if (signInResult.IsLockedOut)
            return Result<AuthResponse>.Failure(_localizer["api.auth.login.locked-out"]);

        if (signInResult.RequiresTwoFactor)
        {
            return Result<AuthResponse>.Success(new AuthResponse
            {
                RequiresTwoFactor = true,
                UserId = user.Id
            });
        }

        if (!signInResult.Succeeded)
            return Result<AuthResponse>.Failure(_localizer["api.auth.login.invalid-credentials"]);

        var tenant = await _tenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == user.TenantId, cancellationToken);

        if (tenant == null || !tenant.IsActive || tenant.Deleted)
            return Result<AuthResponse>.Failure(_localizer["api.auth.login.invalid-tenant"]);

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

        return Result<AuthResponse>.Success(response);
    }
}
