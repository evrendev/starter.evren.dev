using EvrenDev.Application.Identity.Roles.Commands.Create;
using EvrenDev.Application.Identity.Roles.Commands.Update;
using EvrenDev.Application.Identity.Roles.Entities;
using EvrenDev.Application.Identity.Roles.Interfaces;
using EvrenDev.Application.Identity.Roles.Queries.Paginate;

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
    public async Task<PaginationResponse<RoleDto>> GetPaginatedListAsync([FromQuery] PaginateRolesFilter filter)
    {
        return await roleService.PaginatedListAsync(filter);
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
    public Task<RoleDto> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        return roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
    }

    [HttpPut("{id}/permissions")]
    [MustHavePermission(ApiAction.Update, ApiResource.RoleClaims)]
    [OpenApiOperation("Update a role's permissions.", "")]
    public async Task<ActionResult<string>> UpdatePermissionsAsync(string id, UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
        {
            return BadRequest();
        }

        return Ok(await roleService.UpdatePermissionsAsync(request, cancellationToken));
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Roles)]
    [OpenApiOperation("Create or update a role.", "")]
    public Task<string> RegisterRoleAsync(CreateOrUpdateRoleRequest request)
    {
        return roleService.CreateOrUpdateAsync(request);
    }

    [HttpDelete("{id}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Roles)]
    [OpenApiOperation("Delete a role.", "")]
    public Task<string> DeleteAsync(string id)
    {
        return roleService.DeleteAsync(id);
    }
}
