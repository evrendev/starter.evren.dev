using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments;

// Shared by EnrollInCourseRequestHandler (free courses) and
// CapturePaymentOrderRequestHandler / the PayPal webhook job (paid courses,
// after a successful capture) — see Task Q1. Keeps the actual enrollment-row
// shape in exactly one place regardless of which path created it.
public static class CourseEnrollmentFactory
{
    public static CourseEnrollment Create(string userId, Guid courseId, decimal pricePaid)
    {
        return new CourseEnrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            PricePaid = pricePaid,
            PercentComplete = 0
        };
    }
}
