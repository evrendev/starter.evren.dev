using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Queries.Upgrade;

public class UpgradeSubscriptionRequest : IRequest<string>
{
    public string TenantId { get; set; } = default!;
    public DateTime ExtendedExpiryDate { get; set; }
}

public class UpgradeSubscriptionRequestValidator : CustomValidator<UpgradeSubscriptionRequest>
{
    public UpgradeSubscriptionRequestValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}

public class UpgradeSubscriptionRequestHandler(ITenantService tenantService) : IRequestHandler<UpgradeSubscriptionRequest, string>
{
    public Task<string> Handle(UpgradeSubscriptionRequest request, CancellationToken cancellationToken) =>
        tenantService.UpdateSubscription(request.TenantId, request.ExtendedExpiryDate);
}
