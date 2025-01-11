using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Commands.CreateRole;
public class CreateRoleCommand : IRequest<Result<string>>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    private readonly IStringLocalizer<CreateRoleCommandValidator> _localizer;
    public CreateRoleCommandValidator(IStringLocalizer<CreateRoleCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_localizer["api.roles.create.name.required"])
            .MaximumLength(200).WithMessage(_localizer["api.roles.create.name.maxlength"]);
    }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<string>>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public CreateRoleCommandHandler(
        RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new IdentityRole(request.Name);
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return Result<string>.Failure(result.Errors.Select(e => e.Description).ToArray());

        return Result<string>.Success(role.Id);
    }
}
