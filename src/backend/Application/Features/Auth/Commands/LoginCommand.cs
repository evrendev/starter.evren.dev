using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly IStringLocalizer<LoginCommandValidator> _localizer;
    public LoginCommandValidator(IStringLocalizer<LoginCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_localizer["api.auth.login.email.required"])
            .EmailAddress().WithMessage(_localizer["api.auth.login.email.invalid"]);

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage(_localizer["api.auth.login.password.required"]);
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;
    private readonly IStringLocalizer<LoginCommandHandler> _localizer;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IPermissionService permissionService,
        IStringLocalizer<LoginCommandHandler> localizer)
    {
        _userManager = userManager;
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

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
            return Result<AuthResponse>.Failure(_localizer["api.auth.login.invalid-credentials"]);

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
                UserName = user.UserName ?? string.Empty,
                Email = user.Email!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                TenantId = user.TenantId,
                Deleted = user.Deleted,
                Permissions = permissions
            }
        };

        return Result<AuthResponse>.Success(response);
    }
}
