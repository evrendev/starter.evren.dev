using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest<Result<string>>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<string>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionService _permissionService;

        public UpdateRoleCommandHandler(
            RoleManager<IdentityRole> roleManager,
            IPermissionService permissionService)
        {
            _roleManager = roleManager;
            _permissionService = permissionService;
        }

        public async Task<Result<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id);
            if (role == null)
            {
                return Result<string>.Failure(new[] { "Role not found" });
            }

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                return Result<string>.Failure(result.Errors.Select(e => e.Description).ToArray());
            }

            await _permissionService.UpdateRolePermissions(role.Id, request.Permissions);

            return Result<string>.Success(role.Id);
        }
    }
}