using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(v => v.Password)
            .NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _permissionService = permissionService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result<AuthResponse>.Failure("Invalid credentials");

        if (!user.Deleted)
            return Result<AuthResponse>.Failure("User account is deleted");

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
            return Result<AuthResponse>.Failure("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _permissionService.GetUserPermissions(user.Id);

        var token = await _tokenService.GenerateJwtTokenAsync(user, roles, permissions);
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
                Roles = roles.ToList(),
                Permissions = permissions.ToList()
            }
        };

        return Result<AuthResponse>.Success(response);
    }
}
