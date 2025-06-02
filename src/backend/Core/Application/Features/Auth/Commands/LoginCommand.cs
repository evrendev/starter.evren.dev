using EvrenDev.Application.Common.Extensions;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Auth.Commands;


public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Response { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly IStringLocalizer<LoginCommandValidator> _localizer;
    private readonly ReCaptcha _reCaptcha;
    public LoginCommandValidator(IStringLocalizer<LoginCommandValidator> localizer,
        ReCaptcha reCaptcha)
    {
        _localizer = localizer;
        _reCaptcha = reCaptcha;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_localizer["api.auth.login.email.required"])
            .EmailAddress().WithMessage(_localizer["api.auth.login.email.invalid"]);

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage(_localizer["api.auth.login.password.required"]);

        // RuleFor(v => v.Response)
        //     .NotEmpty().NotNull().WithMessage(_localizer["api.auth.login.captcha.required"])
        //     .MustAsync(ValidateRecaptcha)
        //     .WithMessage(_localizer["api.auth.login.captcha.error"])
        //     .WithErrorCode("not_verified");
    }

    public async Task<bool> ValidateRecaptcha(string? response, CancellationToken cancellationToken)
    {
        return await _reCaptcha.IsValid(response ?? string.Empty);
    }
}

public class LoginCommandResponse
{
    public AuthResponse? AuthResponse { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string? UserId { get; set; }
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

    public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null || user.Deleted)
                return Result<AuthResponse>.Failure(_localizer["api.auth.login.invalid-credentials"]);

            var isValid = await _userManager.CheckPasswordAsync(user, command.Password);
            if (!isValid)
                return Result<AuthResponse>.Failure(_localizer["api.auth.login.invalid-credentials"]);

            var existingClaims = await _userManager.GetClaimsAsync(user);
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
                    Gender = user.Gender?.Code,
                    Email = user.Email!,
                    FirstName = user.FirstName!,
                    LastName = user.LastName!,
                    FullName = user.FullName,
                    Image = user.Image,
                    JobTitle = user.JobTitle ?? string.Empty,
                    Language = user.Language?.Code,
                    Permissions = permissions,
                    TwoFactorEnabled = user.TwoFactorEnabled
                }
            };

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<AuthResponse>.Failure(ex.Message);
        }
    }
}
