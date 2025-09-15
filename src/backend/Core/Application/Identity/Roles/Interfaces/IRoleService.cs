using EvrenDev.Application.Identity.Roles.Commands.Create;
using EvrenDev.Application.Identity.Roles.Commands.Update;
using EvrenDev.Application.Identity.Roles.Entities;
using EvrenDev.Application.Identity.Roles.Queries.Paginate;

namespace EvrenDev.Application.Identity.Roles.Interfaces;

public interface IRoleService : ITransientService
{
    Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken);

    Task<PaginationResponse<RoleDto>> PaginatedListAsync(PaginateRolesFilter filter, CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string roleName, string? excludeId);

    Task<RoleDto> GetByIdAsync(string id);

    Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request);

    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken);

    Task<string> DeleteAsync(string id);
}
