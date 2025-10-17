using EvrenDev.Application.Identity.Roles.Commands.Create;
using EvrenDev.Application.Identity.Roles.Commands.Update;
using EvrenDev.Application.Identity.Roles.Entities;
using EvrenDev.Application.Identity.Roles.Interfaces;
using EvrenDev.Application.Identity.Roles.Queries.Paginate;
using StackExchange.Redis;

namespace EvrenDev.PublicApi.Controllers.Identity;

public class RolesController(IRoleService roleService) : VersionNeutralApiController
{
    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Roles)]
    [OpenApiOperation("Get a list of all roles.", "")]
    public async Task<ApiResponse<List<RoleDto>?>> GetListAsync(CancellationToken cancellationToken)
    {
        var data = await roleService.GetListAsync(cancellationToken);
        return ApiResponse<List<RoleDto>?>.Success(data);
    }

    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Roles)]
    [OpenApiOperation("Get paginated list of all roles.", "")]
    public async Task<PaginationResponse<RoleDto>> GetPaginatedListAsync([FromQuery] PaginateRolesFilter filter,
        CancellationToken cancellationToken)
    {
        return await roleService.PaginatedListAsync(filter, cancellationToken);
    }

    [HttpGet("{id}")]
    [MustHavePermission(ApiAction.View, ApiResource.Roles)]
    [OpenApiOperation("Get role details.", "")]
    public async Task<ApiResponse<RoleDto>> GetByIdAsync(string id)
    {
        var data = await roleService.GetByIdAsync(id);
        if (data == null)
            throw new NotFoundException($"Role with ID '{id}' not found.");

        return ApiResponse<RoleDto>.Success(data);
    }

    [HttpGet("{id}/permissions")]
    [MustHavePermission(ApiAction.View, ApiResource.RoleClaims)]
    [OpenApiOperation("Get role details with its permissions.", "")]
    public async Task<ApiResponse<RoleDto>> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        var response = await roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
        if (response == null)
            throw new NotFoundException($"Role with ID '{id}' not found.");

        return ApiResponse<RoleDto>.Success(response);
    }

    [HttpPut("{id}/permissions")]
    [MustHavePermission(ApiAction.Update, ApiResource.RoleClaims)]
    [OpenApiOperation("Update a role's permissions.", "")]
    public async Task<ActionResult<string>> UpdatePermissionsAsync(string id, UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
            return BadRequest();

        return Ok(await roleService.UpdatePermissionsAsync(request, cancellationToken));
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Roles)]
    [OpenApiOperation("Create or update a role.", "")]
    public async Task<ApiResponse<string?>> RegisterRoleAsync(CreateOrUpdateRoleRequest request)
    {
        return ApiResponse<string?>.Success(await roleService.CreateOrUpdateAsync(request));
    }

    [HttpPut("{id}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Roles)]
    [OpenApiOperation("Create or update a role.", "")]
    public async Task<ApiResponse<string?>> UpdateRoleAsync(CreateOrUpdateRoleRequest request)
    {
        return ApiResponse<string?>.Success(await roleService.CreateOrUpdateAsync(request));
    }

    [HttpDelete("{id}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Roles)]
    [OpenApiOperation("Delete a role.", "")]
    public async Task<ApiResponse<string?>> DeleteAsync(string id)
    {
        return ApiResponse<string?>.Success(await roleService.DeleteAsync(id));
    }
}
