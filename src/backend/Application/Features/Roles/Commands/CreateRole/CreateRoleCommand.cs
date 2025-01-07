using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<Result<string>>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
        }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<string>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionService _permissionService;

        public CreateRoleCommandHandler(
            RoleManager<IdentityRole> roleManager,
            IPermissionService permissionService)
        {
            _roleManager = roleManager;
            _permissionService = permissionService;
        }

        public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new IdentityRole(request.Name);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return Result<string>.Failure(result.Errors.Select(e => e.Description).ToArray());
            }

            return Result<string>.Success(role.Id);
        }
    }
}
