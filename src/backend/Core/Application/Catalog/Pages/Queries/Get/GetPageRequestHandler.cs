using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Identity.Users.Interfaces;
using EvrenDev.Domain.Catalog;
using EvrenDev.Shared.Authorization;

namespace EvrenDev.Application.Catalog.Pages.Queries.Get;

public class GetPageRequest(Guid id) : IRequest<PageDetailsDto>
{
    public Guid Id { get; set; } = id;
}

// "Pages.View" is IsBasic (see ApiPermissions.cs), so this admin-CRUD endpoint is
// technically reachable by any authenticated user, including plain students — it returns
// the raw page Content directly (unlike the player endpoint, no enrollment gate at all).
// The staging check below closes the same-class leak this whole task is about; it does NOT
// add the (separate, much larger, pre-existing) enrollment gate this endpoint is still
// missing for published content — that's out of scope here (see PPTX import Task H report).
public class GetPageRequestHandler(
    IRepository<Page> repository,
    ICurrentUser currentUser,
    IUserService userService,
    IStringLocalizer<GetPageRequestHandler> localizer)
    : IRequestHandler<GetPageRequest, PageDetailsDto>
{
    public async Task<PageDetailsDto> Handle(GetPageRequest request, CancellationToken cancellationToken)
    {
        var page = await repository.FirstOrDefaultAsync(
            new PageByIdWithChapterSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(string.Format(localizer["catalog.pages.get.notfound"], request.Id));

        // This app's JWTs carry no role claim (see PPTX import Task H) — GetRolesAsync does
        // the same live DB (IsInRoleAsync) check the admin Users screen uses. A permission-
        // claim check was rejected: in this tenant's live role config "Basic" has been
        // granted extra permissions that made it indistinguishable from Admin for that purpose.
        var roles = await userService.GetRolesAsync(currentUser.GetUserId().ToString(), cancellationToken);
        var isAdmin = roles.Any(r => r.Enabled && r.RoleName == ApiRoles.Admin);

        if (!isAdmin && page.Chapter.IsStaging)
            throw new ForbiddenException("This content has not been published yet.");

        return page;
    }
}
