using EvrenDev.Application.Catalog.CourseEnrollments.Entities;
using EvrenDev.Application.Catalog.CourseEnrollments.Queries.Enroll;
using EvrenDev.Application.Catalog.CourseEnrollments.Queries.GetMyEnrollments;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class CourseEnrollmentsController : VersionedApiController
{
    [HttpPost]
    [Authorize]
    [OpenApiOperation("Enroll the current user in a course.", "")]
    public Task<bool> EnrollAsync(EnrollInCourseRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("mine")]
    [Authorize]
    [OpenApiOperation("Get the current user's course enrollments.", "")]
    public async Task<ApiResponse<List<CourseEnrollmentDto>>> GetMyEnrollmentsAsync()
    {
        var data = await Mediator.Send(new GetMyEnrollmentsRequest());

        return ApiResponse<List<CourseEnrollmentDto>>.Success(data);
    }
}
