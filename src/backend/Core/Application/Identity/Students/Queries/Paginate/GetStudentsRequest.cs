using EvrenDev.Application.Common.Models;

namespace EvrenDev.Application.Identity.Students.Queries.Paginate;

// Mirrors PaginateUsersFilter's shape (Identity's paginated-list convention
// goes through IUserService/IStudentService directly rather than MediatR,
// since it queries UserManager.Users, not a repository) — see Task R1.
public class GetStudentsRequest : PaginationFilter
{
    public bool? IsActive { get; set; }
}
