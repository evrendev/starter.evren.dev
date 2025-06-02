using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Queries.Deactivate;

public class DeactivateTenantRequest : IRequest<string>
{
    public string TenantId { get; set; } = default!;

    public DeactivateTenantRequest(string tenantId) => TenantId = tenantId;
}

public class DeactivateTenantRequestValidator : CustomValidator<DeactivateTenantRequest>
{
    public DeactivateTenantRequestValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}

public class DeactivateTenantRequestHandler(ITenantService tenantService) : IRequestHandler<DeactivateTenantRequest, string>
{
    public Task<string> Handle(DeactivateTenantRequest request, CancellationToken cancellationToken) =>
        tenantService.DeactivateAsync(request.TenantId);
}
