using EvrenDev.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<Result<bool>>
{
    public string Id { get; set; } = string.Empty;
}

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    private readonly IStringLocalizer<DeleteRoleCommandValidator> _localizer;

    public DeleteRoleCommandValidator(IStringLocalizer<DeleteRoleCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.roles.delete.id-required"]);
    }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IStringLocalizer<DeleteRoleCommandHandler> _localizer;

    public DeleteRoleCommandHandler(
        RoleManager<IdentityRole> roleManager,
        IStringLocalizer<DeleteRoleCommandHandler> localizer)
    {
        _roleManager = roleManager;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null)
        {
            var notFoundMessage = _localizer["api.roles.not-found"];
            return Result<bool>.Failure(new[] { notFoundMessage.ToString() });
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            return Result<bool>.Failure(result.Errors.Select(e => e.Description).ToArray());

        return Result<bool>.Success(true);
    }
}
