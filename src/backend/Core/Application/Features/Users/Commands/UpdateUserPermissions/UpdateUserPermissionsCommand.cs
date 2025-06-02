using System.Security.Claims;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Users.Commands.UpdateUserPermissions;

public record UpdateUserPermissionsCommand : IRequest<Result<bool>>
{
    public string Id { get; init; } = string.Empty;
    public List<string> Permissions { get; init; } = new();
}

public class UpdateUserPermissionsCommandValidator : AbstractValidator<UpdateUserPermissionsCommand>
{
    private readonly IStringLocalizer<UpdateUserPermissionsCommandValidator> _localizer;

    public UpdateUserPermissionsCommandValidator(
        IStringLocalizer<UpdateUserPermissionsCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.users.update-permissions.id.required"]);

        RuleFor(v => v.Permissions)
            .NotNull().WithMessage(_localizer["api.users.update-permissions.permissions.required"]);
    }
}

public class UpdateUserPermissionsCommandHandler : IRequestHandler<UpdateUserPermissionsCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<UpdateUserPermissionsCommandHandler> _localizer;

    public UpdateUserPermissionsCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<UpdateUserPermissionsCommandHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(UpdateUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
            return Result<bool>.Failure(_localizer["api.users.not-found"].Value);

        var currentClaims = await _userManager.GetClaimsAsync(user);
        var currentPermissions = currentClaims.Where(c => c.Type == "permission").ToList();

        // Remove all current permission claims
        var removeResult = await _userManager.RemoveClaimsAsync(user, currentPermissions);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e => e.Description).ToArray();
            return Result<bool>.Failure(errors);
        }

        // Add new permission claims
        if (request.Permissions.Any())
        {
            var newClaims = request.Permissions.Select(p => new Claim("permission", p));
            var addResult = await _userManager.AddClaimsAsync(user, newClaims);
            if (!addResult.Succeeded)
            {
                var errors = addResult.Errors.Select(e => e.Description).ToArray();
                return Result<bool>.Failure(errors);
            }
        }

        return Result<bool>.Success(true);
    }
}
