using EvrenDev.Application.Multitenancy.Entities;
using EvrenDev.Application.Multitenancy.Commands.Active;
using EvrenDev.Application.Multitenancy.Commands.Create;
using EvrenDev.Application.Multitenancy.Commands.Deactivate;
using EvrenDev.Application.Multitenancy.Queries.Get;
using EvrenDev.Application.Multitenancy.Queries.GetAll;
using EvrenDev.Application.Multitenancy.Commands.Upgrade;
using EvrenDev.Application.Multitenancy.Commands.Update;

namespace EvrenDev.PublicApi.Controllers.Multitenancy;

public class TenantsController : VersionNeutralApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Tenants)]
    [OpenApiOperation("Get a list of all tenants.", "")]
    public Task<List<TenantDto>> GetListAsync()
    {
        return Mediator.Send(new GetAllTenantsRequest());
    }

    [HttpGet("{id}")]
    [MustHavePermission(ApiAction.View, ApiResource.Tenants)]
    [OpenApiOperation("Get tenant details.", "")]
    public Task<TenantDto> GetAsync(string id)
    {
        return Mediator.Send(new GetTenantRequest(id));
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Tenants)]
    [OpenApiOperation("Create a new tenant.", "")]
    public Task<string> CreateAsync(CreateTenantCommand command)
    {
        return Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Update a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<string?>> UpdateAsync(string id, UpdateTenantCommand command)
    {
        if (!string.Equals(id, command.Id, StringComparison.OrdinalIgnoreCase))
            return ApiResponse<string?>.Failure("Invalid tenant ID.");

        var data = await Mediator.Send(command);

        if (string.IsNullOrEmpty(data))
            throw new NotFoundException($"Tenant with ID '{id}' not found.");

        return ApiResponse<string?>.Success(data);
    }

    [HttpPost("{id}/activate")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Activate a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public Task<string> ActivateAsync(string id)
    {
        return Mediator.Send(new ActivateTenantCommand(id));
    }

    [HttpPost("{id}/deactivate")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Deactivate a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public Task<string> DeactivateAsync(string id)
    {
        return Mediator.Send(new DeactivateTenantCommand(id));
    }

    [HttpPost("{id}/upgrade")]
    [MustHavePermission(ApiAction.UpgradeSubscription, ApiResource.Tenants)]
    [OpenApiOperation("Upgrade a tenant's subscription.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ActionResult<string>> UpgradeSubscriptionAsync(string id, UpgradeSubscriptionCommand command)
    {
        return id != command.TenantId
            ? BadRequest()
            : Ok(await Mediator.Send(command));
    }
}
