using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Specifications;

public class CourseEnrollmentByUserAndCourseSpec : Specification<CourseEnrollment>, ISingleResultSpecification<CourseEnrollment>
{
    public CourseEnrollmentByUserAndCourseSpec(string userId, Guid courseId) =>
        Query.Where(e => e.UserId == userId && e.CourseId == courseId);
}
