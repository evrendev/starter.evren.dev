using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Queries.Active;

public class ActivateTenantRequest : IRequest<string>
{
    public string TenantId { get; set; } = default!;

    public ActivateTenantRequest(string tenantId) => TenantId = tenantId;
}

public class ActivateTenantRequestValidator : CustomValidator<ActivateTenantRequest>
{
    public ActivateTenantRequestValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}
public class ActivateTenantRequestHandler(ITenantService tenantService) : IRequestHandler<ActivateTenantRequest, string>
{
    public Task<string> Handle(ActivateTenantRequest request, CancellationToken cancellationToken) =>
        tenantService.ActivateAsync(request.TenantId);
}
