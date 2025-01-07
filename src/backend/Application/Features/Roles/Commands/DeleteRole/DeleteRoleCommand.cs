using EvrenDev.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<Result<bool>>
{
    public string Id { get; set; } = string.Empty;
}

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public DeleteRoleCommandHandler(
        RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            return Result<bool>.Failure(new[] { "Role not found" });
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return Result<bool>.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        return Result<bool>.Success(true);
    }
}
