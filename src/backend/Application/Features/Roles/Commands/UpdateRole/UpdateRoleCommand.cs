using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Commands.UpdateRole;
public class UpdateRoleCommand : IRequest<Result<string>>
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    private readonly IStringLocalizer<UpdateRoleCommandValidator> _localizer;

    public UpdateRoleCommandValidator(IStringLocalizer<UpdateRoleCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.roles.update.id-required"]);

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_localizer["api.roles.update.name.required"])
            .MaximumLength(50).WithMessage(_localizer["api.roles.update.name.maxlength"]);
    }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<string>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IStringLocalizer<UpdateRoleCommandHandler> _localizer;

    public UpdateRoleCommandHandler(
        RoleManager<IdentityRole> roleManager,
        IStringLocalizer<UpdateRoleCommandHandler> localizer)
    {
        _roleManager = roleManager;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            var notFoundMessage = _localizer["api.roles.not-found"];
            return Result<string>.Failure(new[] { notFoundMessage.ToString() });
        }

        role.Name = request.Name;
        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return Result<string>.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        return Result<string>.Success(role.Id);
    }
}
