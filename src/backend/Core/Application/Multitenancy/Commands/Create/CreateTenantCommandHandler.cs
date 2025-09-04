using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Create;

public class CreateTenantCommand : IRequest<string>
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public string? Issuer { get; set; }
}

public class CreateTenantCommandValidator : CustomValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator(
        ITenantService tenantService,
        IStringLocalizer<CreateTenantCommandValidator> localizer,
        IConnectionStringValidator connectionStringValidator)
    {
        RuleFor(t => t.Id).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (id, _) => !await tenantService.ExistsWithIdAsync(id))
                .WithMessage((_, id) => string.Format(localizer["tenant.alreadyexists"], id));

        RuleFor(t => t.Name).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await tenantService.ExistsWithNameAsync(name!))
                .WithMessage((_, name) => string.Format(localizer["tenant.alreadyexists"], name));

        RuleFor(t => t.ConnectionString).Cascade(CascadeMode.Stop)
            .Must((_, cs) => string.IsNullOrWhiteSpace(cs) || connectionStringValidator.TryValidate(cs))
                .WithMessage(localizer["invalid.connectionstring"]);

        RuleFor(t => t.AdminEmail).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
    }
}

public class CreateTenantCommandHandler(ITenantService tenantService) : IRequestHandler<CreateTenantCommand, string>
{
    public Task<string> Handle(CreateTenantCommand command, CancellationToken cancellationToken) =>
        tenantService.CreateAsync(command, cancellationToken);
}
