using EvrenDev.Application.Catalog.CourseEnrollments.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Specifications;

public class MyCourseEnrollmentsSpec : Specification<CourseEnrollment, CourseEnrollmentDto>
{
    public MyCourseEnrollmentsSpec(string userId) =>
        Query
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
            .ThenInclude(c => c.Category);
}
