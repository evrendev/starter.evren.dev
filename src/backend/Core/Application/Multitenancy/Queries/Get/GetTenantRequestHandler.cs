using EvrenDev.Application.Multitenancy.Entities;
using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Queries.Get;

public class GetTenantRequest : IRequest<TenantDto>
{
    public string TenantId { get; set; } = default!;

    public GetTenantRequest(string tenantId) => TenantId = tenantId;
}
public class GetTenantRequestValidator : CustomValidator<GetTenantRequest>
{
    public GetTenantRequestValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}

public class GetTenantRequestHandler(ITenantService tenantService) : IRequestHandler<GetTenantRequest, TenantDto>
{
    public Task<TenantDto> Handle(GetTenantRequest request, CancellationToken cancellationToken) =>
        tenantService.GetByIdAsync(request.TenantId);
}
