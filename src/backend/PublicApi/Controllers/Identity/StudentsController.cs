using EvrenDev.Application.Catalog.CourseEnrollments.Queries.AdminEnroll;
using EvrenDev.Application.Catalog.CourseEnrollments.Queries.RemoveEnrollment;
using EvrenDev.Application.Identity.Students.Entities;
using EvrenDev.Application.Identity.Students.Interfaces;
using EvrenDev.Application.Identity.Students.Queries.Paginate;

namespace EvrenDev.PublicApi.Controllers.Identity;

public class StudentsController(IStudentService studentService) : VersionNeutralApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Students)]
    [OpenApiOperation("Get paginated list of students, with enrollment/payment/progress summary.", "")]
    public async Task<PaginationResponse<StudentSummaryDto>> GetPaginatedListAsync([FromQuery] GetStudentsRequest filter,
        CancellationToken cancellationToken)
    {
        return await studentService.PaginatedListAsync(filter, cancellationToken);
    }

    [HttpGet("summary-stats")]
    [MustHavePermission(ApiAction.View, ApiResource.Students)]
    [OpenApiOperation("Get aggregate revenue/completion stats across all students.", "")]
    public async Task<StudentsSummaryStatsDto> GetSummaryStatsAsync(CancellationToken cancellationToken)
    {
        return await studentService.GetSummaryStatsAsync(cancellationToken);
    }

    [HttpPost("{userId}/enrollments/{courseId:guid}")]
    [MustHavePermission(ApiAction.Create, ApiResource.Students)]
    [OpenApiOperation("Manually enroll a student into a course, bypassing payment.", "")]
    public Task<bool> EnrollAsync(string userId, Guid courseId, CancellationToken cancellationToken)
    {
        return Mediator.Send(new AdminEnrollStudentRequest(userId, courseId), cancellationToken);
    }

    [HttpDelete("{userId}/enrollments/{courseId:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Students)]
    [OpenApiOperation("Remove a student's enrollment from a course.", "")]
    public Task<bool> RemoveEnrollmentAsync(string userId, Guid courseId, CancellationToken cancellationToken)
    {
        return Mediator.Send(new RemoveEnrollmentRequest(userId, courseId), cancellationToken);
    }
}
