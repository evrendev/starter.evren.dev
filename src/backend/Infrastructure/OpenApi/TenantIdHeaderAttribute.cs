using EvrenDev.Shared.Multitenancy;

namespace EvrenDev.Infrastructure.OpenApi;

public class TenantIdHeaderAttribute() : SwaggerHeaderAttribute(MultitenancyConstants.TenantIdName,
    "Input your tenant Id to access this API",
    string.Empty,
    true);
