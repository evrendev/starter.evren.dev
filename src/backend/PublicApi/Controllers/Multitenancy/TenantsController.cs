using EvrenDev.Application.Multitenancy.Commands.Active;
using EvrenDev.Application.Multitenancy.Commands.Create;
using EvrenDev.Application.Multitenancy.Commands.Deactivate;
using EvrenDev.Application.Multitenancy.Commands.Delete;
using EvrenDev.Application.Multitenancy.Commands.Update;
using EvrenDev.Application.Multitenancy.Commands.Upgrade;
using EvrenDev.Application.Multitenancy.Entities;
using EvrenDev.Application.Multitenancy.Queries.Get;
using EvrenDev.Application.Multitenancy.Queries.GetAll;
using EvrenDev.Application.Multitenancy.Queries.Paginate;

namespace EvrenDev.PublicApi.Controllers.Multitenancy;

public class TenantsController : VersionNeutralApiController
{
    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Tenants)]
    [OpenApiOperation("Get a list of all tenants.", "")]
    public Task<List<TenantDto>> GetListAsync()
    {
        return Mediator.Send(new GetAllTenantsRequest());
    }

    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Tenants)]
    [OpenApiOperation("Get paginated list of tenants.", "")]
    public async Task<PaginationResponse<TenantDto>> GetPaginatedListAsync([FromQuery] PaginateTenantsFilter filter)
    {
        return await Mediator.Send(filter);
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
    public async Task<ApiResponse<string?>> CreateAsync(CreateTenantCommand command)
    {
        return ApiResponse<string?>.Success(await Mediator.Send(command));
    }

    [HttpPut("{id}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Update a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<string?>> UpdateAsync(string id, UpdateTenantCommand command)
    {
        if (!string.Equals(id, command.Id, StringComparison.OrdinalIgnoreCase))
            return ApiResponse<string?>.Failure("Invalid tenant ID.");

        return ApiResponse<string?>.Success(await Mediator.Send(command));
    }

    //Note: Add tenants delete permission to user permission
    [HttpDelete("{id}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Delete a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        return ApiResponse<bool>.Success(await Mediator.Send(new DeleteTenantCommand(id)));
    }

    [HttpPost("{id}/activate")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Activate a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<string>> ActivateAsync(string id)
    {
        return ApiResponse<string>.Success(await Mediator.Send(new ActivateTenantCommand(id)));
    }

    [HttpPost("{id}/deactivate")]
    [MustHavePermission(ApiAction.Update, ApiResource.Tenants)]
    [OpenApiOperation("Deactivate a tenant.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<string>> DeactivateAsync(string id)
    {
        return ApiResponse<string>.Success(await Mediator.Send(new DeactivateTenantCommand(id)));
    }

    [HttpPost("{id}/upgrade")]
    [MustHavePermission(ApiAction.UpgradeSubscription, ApiResource.Tenants)]
    [OpenApiOperation("Upgrade a tenant's subscription.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<string>> UpgradeSubscriptionAsync(string id, UpgradeSubscriptionCommand command)
    {
        return id != command.TenantId
            ? ApiResponse<string>.Failure("Invalid tenant ID.")
            : ApiResponse<string>.Success(await Mediator.Send(command));
    }
}
