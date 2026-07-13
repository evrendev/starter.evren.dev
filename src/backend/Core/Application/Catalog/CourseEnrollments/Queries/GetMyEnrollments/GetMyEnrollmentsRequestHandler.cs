using EvrenDev.Application.Catalog.CourseEnrollments.Entities;
using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Queries.GetMyEnrollments;

public class GetMyEnrollmentsRequest : IRequest<List<CourseEnrollmentDto>>
{
}

public class GetMyEnrollmentsRequestHandler(
    IReadRepository<CourseEnrollment> repository,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyEnrollmentsRequest, List<CourseEnrollmentDto>>
{
    public async Task<List<CourseEnrollmentDto>> Handle(GetMyEnrollmentsRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId().ToString();

        return await repository.ListAsync(new MyCourseEnrollmentsSpec(userId), cancellationToken);
    }
}
