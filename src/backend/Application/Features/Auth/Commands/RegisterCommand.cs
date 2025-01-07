using System.Text.RegularExpressions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Shared.Constants;
using EvrenDev.Shared.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string Gender { get; set; } = Defaults.Gender.Code;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string Language { get; set; } = Defaults.Language.Code;
    public string TenantId { get; set; } = string.Empty;
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IStringLocalizer<RegisterCommandValidator> _localizer;
    private readonly UserManager<ApplicationUser> _userManager;
    public RegisterCommandValidator(IStringLocalizer<RegisterCommandValidator> localizer,
        UserManager<ApplicationUser> userManager)
    {
        _localizer = localizer;
        _userManager = userManager;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_localizer["api.auth.register.email.required"])
            .EmailAddress().WithMessage(_localizer["api.auth.register.email.invalid"])
            .MaximumLength(500).WithMessage(_localizer["api.auth.register.email.maxlength"])
            .MustAsync(BeUniqueEmail)
                .WithMessage(_localizer["api.auth.register.email.exists"])
                .WithErrorCode("exists"); ;

        RuleFor(v => v.Password)
            .Must(password => !CheckPasswordComplexity(password)).WithMessage(_localizer["api.auth.register.password.complexity"]);

        RuleFor(e => e.ConfirmPassword)
            .Equal(e => e.Password).WithMessage(_localizer["api.auth.register.password.equality"]);

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage(_localizer["api.auth.register.first-name.required"])
            .MaximumLength(100).WithMessage(_localizer["api.auth.register.first-name.maxlength"]);

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage(_localizer["api.auth.register.last-name.required"])
            .MaximumLength(100).WithMessage(_localizer["api.auth.register.last-name.maxlength"]);

        RuleFor(v => v.TenantId)
            .NotEmpty().WithMessage(_localizer["api.auth.register.tenant-id.required"]);
    }

    public async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => string.Equals(email, u.Email, StringComparison.OrdinalIgnoreCase), cancellationToken);

        return user == null;
    }

    protected bool CheckPasswordComplexity(string? password)
    {
        if (password == null) return false;

        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMinimum6Chars = new Regex(@".{6,}");
        var hasMaximum20Chars = new Regex(@".{20,}");
        var hasLowerChar = new Regex(@"[a-z]+");
        var hasSpecialCharacter = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        if (!hasLowerChar.IsMatch(password))
            return false;

        if (!hasUpperChar.IsMatch(password))
            return false;

        if (!hasNumber.IsMatch(password))
            return false;

        if (!hasSpecialCharacter.IsMatch(password))
            return false;

        if (!hasMinimum6Chars.IsMatch(password))
            return false;

        if (!hasMaximum20Chars.IsMatch(password))
            return false;

        return true;
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _permissionService = permissionService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            TenantId = request.TenantId,
            UserName = request.Email,
            Email = request.Email,
            Gender = Gender.From(request.Gender),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Image = request.Image,
            JobTitle = request.JobTitle,
            Language = Language.From(request.Language),
            Deleted = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result<AuthResponse>.Failure(result.Errors.Select(e => e.Description).ToArray());

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
                UserName = user.UserName,
                Email = user.Email!,
                Gender = user.Gender,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobTitle = user.JobTitle ?? string.Empty,
                Image = user.Image,
                Language = user.Language,
                Deleted = user.Deleted,
                Permissions = permissions
            }
        };

        return Result<AuthResponse>.Success(response);
    }
}
