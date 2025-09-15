using EvrenDev.Application.Identity.Users.Entities;

namespace EvrenDev.Application.Identity.Users.Queries.Paginate;

public class PaginateUsersFilter : PaginationFilter, IRequest<PaginationResponse<UserDto>>
{
    public bool? ShowActiveItems { get; set; }
}
