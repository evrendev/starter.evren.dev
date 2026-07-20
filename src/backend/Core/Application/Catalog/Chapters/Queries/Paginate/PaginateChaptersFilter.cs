using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Identity.Users.Interfaces;
using EvrenDev.Domain.Catalog;
using EvrenDev.Shared.Authorization;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Paginate;

public class PaginateChaptersFilter : PaginationFilter, IRequest<PaginationResponse<ChapterDto>>
{
    public Guid? CourseId { get; set; }
    public bool? Published { get; set; }
}

public class PaginateChaptersFilterHandler(
    IReadRepository<Chapter> repository,
    ICurrentUser currentUser,
    IUserService userService) : IRequestHandler<PaginateChaptersFilter, PaginationResponse<ChapterDto>>
{
    public async Task<PaginationResponse<ChapterDto>> Handle(PaginateChaptersFilter request, CancellationToken cancellationToken)
    {
        // "Chapters.View" is IsBasic (every authenticated user has it, including plain
        // enrolled students — see ApiPermissions.cs), so the only reliable Admin-vs-student
        // signal is actual Admin ROLE membership. NOTE: this app's JWTs carry no role claim
        // at all (confirmed empirically — see PPTX import Task H), so ICurrentUser.IsInRole()
        // always returns false; a permission-claim check was also tried and rejected — in
        // this tenant's live role config the "Basic" role has been granted extra permissions
        // (e.g. Chapters.Update) that made it indistinguishable from Admin for that purpose.
        // GetRolesAsync does the same live DB (IsInRoleAsync) check the admin Users screen uses.
        var roles = await userService.GetRolesAsync(currentUser.GetUserId().ToString(), cancellationToken);
        var includeStaging = roles.Any(r => r.Enabled && r.RoleName == ApiRoles.Admin);
        var spec = new ChaptersBySearchRequestWithCoursesSpec(request, includeStaging);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
