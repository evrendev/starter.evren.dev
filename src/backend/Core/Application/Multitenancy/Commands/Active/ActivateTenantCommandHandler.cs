using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Active;

public class ActivateTenantCommand : IRequest<string>
{
    public ActivateTenantCommand(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; } = default!;
}

public class ActivateTenantCommandValidator : CustomValidator<ActivateTenantCommand>
{
    public ActivateTenantCommandValidator()
    {
        RuleFor(t => t.TenantId)
            .NotEmpty();
    }
}

public class ActivateTenantCommandHandler(ITenantService tenantService) : IRequestHandler<ActivateTenantCommand, string>
{
    public Task<string> Handle(ActivateTenantCommand command, CancellationToken cancellationToken)
    {
        return tenantService.ActivateAsync(command.TenantId);
    }
}
