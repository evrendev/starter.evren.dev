using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Auth.Commands;

public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public string UserId { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();

        RuleFor(v => v.RefreshToken)
            .NotEmpty();
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _permissionService = permissionService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _tokenService.ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (!isValid)
            return Result<AuthResponse>.Failure("Invalid refresh token");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result<AuthResponse>.Failure("User not found");

        if (user.Deleted)
            return Result<AuthResponse>.Failure("User account is deleted");

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
