using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Queries.Enroll;

public class EnrollInCourseRequest(Guid courseId) : IRequest<bool>
{
    public Guid CourseId { get; set; } = courseId;
}

public class EnrollInCourseRequestValidator : CustomValidator<EnrollInCourseRequest>
{
    public EnrollInCourseRequestValidator(
        IReadRepository<Course> courseRepo,
        IReadRepository<CourseEnrollment> enrollmentRepo,
        ICurrentUser currentUser,
        IStringLocalizer<EnrollInCourseRequestValidator> localizer)
    {
        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));

        RuleFor(p => p.CourseId)
            .MustAsync(async (id, ct) =>
            {
                var userId = currentUser.GetUserId().ToString();
                return await enrollmentRepo.FirstOrDefaultAsync(
                    new CourseEnrollmentByUserAndCourseSpec(userId, id), ct) is null;
            })
                .WithMessage((_, id) => string.Format(localizer["catalog.courseenrollments.create.alreadyexists"], id));
    }
}

public class EnrollInCourseRequestHandler(
    IRepository<CourseEnrollment> repository,
    ICurrentUser currentUser)
    : IRequestHandler<EnrollInCourseRequest, bool>
{
    public async Task<bool> Handle(EnrollInCourseRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId().ToString();

        var existingEnrollment = await repository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, request.CourseId), cancellationToken);

        if (existingEnrollment is not null)
            throw new ConflictException($"You are already enrolled in course '{request.CourseId}'.");

        var enrollment = new CourseEnrollment
        {
            UserId = userId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow,
            PricePaid = 0,
            PercentComplete = 0
        };

        await repository.AddAsync(enrollment, cancellationToken);

        return true;
    }
}
