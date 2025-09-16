using EvrenDev.Application.Identity.Roles.Entities;

namespace EvrenDev.Application.Identity.Roles.Queries.Paginate;

public class PaginateRolesFilter : PaginationFilter, IRequest<PaginationResponse<RoleDto>>
{
    public bool? ShowActiveItems { get; set; }
}
