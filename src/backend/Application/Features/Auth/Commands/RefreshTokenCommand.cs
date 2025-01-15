using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Auth.Commands;

public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public string UserId { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    private readonly IStringLocalizer<RefreshTokenCommandValidator> _localizer;
    public RefreshTokenCommandValidator(IStringLocalizer<RefreshTokenCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage(_localizer["api.auth.refresh-token.user-id.required"]);

        RuleFor(v => v.RefreshToken)
            .NotEmpty().WithMessage(_localizer["api.auth.refresh-token.refresh-token.required"]);
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;
    private readonly IStringLocalizer<RefreshTokenCommandHandler> _localizer;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITenantDbContext tenantDbContext,
        ITokenService tokenService,
        IPermissionService permissionService,
        IStringLocalizer<RefreshTokenCommandHandler> localizer)
    {
        _userManager = userManager;
        _tenantDbContext = tenantDbContext;
        _tokenService = tokenService;
        _permissionService = permissionService;
        _localizer = localizer;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _tokenService.ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (!isValid)
            return Result<AuthResponse>.Failure(_localizer["api.auth.refresh-token.invalid"]);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result<AuthResponse>.Failure(_localizer["api.auth.refresh-token.not-found"]);

        if (user.Deleted)
            return Result<AuthResponse>.Failure(_localizer["api.auth.refresh-token.deleted"]);

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
                Email = user.Email!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                TenantId = user.TenantId,
                Permissions = permissions
            }
        };

        return Result<AuthResponse>.Success(response);
    }
}
