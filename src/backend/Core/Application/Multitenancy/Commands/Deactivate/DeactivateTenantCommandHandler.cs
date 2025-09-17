using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Deactivate;

public class DeactivateTenantCommand : IRequest<string>
{
    public DeactivateTenantCommand(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; } = default!;
}

public class DeactivateTenantCommandValidator : CustomValidator<DeactivateTenantCommand>
{
    public DeactivateTenantCommandValidator()
    {
        RuleFor(t => t.TenantId)
            .NotEmpty();
    }
}

public class DeactivateTenantCommandHandler
    (ITenantService tenantService) : IRequestHandler<DeactivateTenantCommand, string>
{
    public Task<string> Handle(DeactivateTenantCommand command, CancellationToken cancellationToken)
    {
        return tenantService.DeactivateAsync(command.TenantId);
    }
}
