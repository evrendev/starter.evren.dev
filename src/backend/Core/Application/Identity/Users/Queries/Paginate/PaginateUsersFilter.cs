using EvrenDev.Application.Identity.Users.Entities;

namespace EvrenDev.Application.Identity.Users.Queries.Paginate;

public class PaginateUsersFilter : PaginationFilter, IRequest<PaginationResponse<BasicUserDto>>
{
    public bool? IsActive { get; set; }
    public bool? TwoFactorEnabled { get; set; }
}
