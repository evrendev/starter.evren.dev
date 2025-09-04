using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Upgrade;

public class UpgradeSubscriptionCommand : IRequest<string>
{
    public string TenantId { get; set; } = default!;
    public DateTime ExtendedExpiryDate { get; set; }
}

public class UpgradeSubscriptionCommandValidator : CustomValidator<UpgradeSubscriptionCommand>
{
    public UpgradeSubscriptionCommandValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}

public class UpgradeSubscriptionCommandHandler(ITenantService tenantService) : IRequestHandler<UpgradeSubscriptionCommand, string>
{
    public Task<string> Handle(UpgradeSubscriptionCommand command, CancellationToken cancellationToken) =>
        tenantService.UpdateSubscription(command.TenantId, command.ExtendedExpiryDate);
}
