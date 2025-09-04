using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Update;

public class UpdateTenantCommand : IRequest<string>
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime? ValidUpto { get; set; }
    public string? Issuer { get; set; }
}

public class UpdateTenantCommandValidator : CustomValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator(
        ITenantService tenantService,
        IStringLocalizer<UpdateTenantCommandValidator> localizer,
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

        RuleFor(t => t.ValidUpto)
            .Must(date => date is null || date > DateTime.UtcNow)
                .WithMessage(localizer["validupto.greaterthan.now"]);
    }
}

public class UpdateTenantCommandHandler(ITenantService tenantService) : IRequestHandler<UpdateTenantCommand, string>
{
    public Task<string> Handle(UpdateTenantCommand command, CancellationToken cancellationToken) =>
        tenantService.UpdateAsync(command, cancellationToken);
}
