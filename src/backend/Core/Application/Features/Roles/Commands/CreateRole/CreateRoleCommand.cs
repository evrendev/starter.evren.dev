using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Commands.CreateRole;
public class CreateRoleCommand : IRequest<Result<Guid>>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
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

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new ApplicationRole()
        {
            Name = request.Name,
            Description = request.Description,
            Deleted = false
        };
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return Result<Guid>.Failure(result.Errors.Select(e => e.Description).ToArray());

        return Result<Guid>.Success(role.Id);
    }
}
