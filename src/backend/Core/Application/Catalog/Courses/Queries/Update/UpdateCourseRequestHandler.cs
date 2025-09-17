using EvrenDev.Application.Catalog.Courses.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Courses.Queries.Update;

public class UpdateCourseRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Intrudiction { get; set; }
    public string? Description { get; set; }
    public string[]? Tags { get; set; }
    public bool Published { get; set; }
    public bool Upcoming { get; set; }
    public bool Featured { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public bool Paid { get; set; }
    public bool CompletetionCertificate { get; set; }
    public bool PaidCertificate { get; set; }
    public Guid CategoryId { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
    public FileUploadRequest? Image { get; set; }
}

public class UpdateCourseRequestValidator : CustomValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator(IReadRepository<Course> courseRepo, IReadRepository<Category> categoryRepo, IStringLocalizer<UpdateCourseRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (course, name, ct) =>
                    await courseRepo.FirstOrDefaultAsync(new CourseByTitleSpec(name), ct)
                        is not Course existingCourse || existingCourse.Id == course.Id)
                .WithMessage((_, name) => string.Format(localizer["catalog.courses.update.alreadyexists"], name));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());

        RuleFor(p => p.CategoryId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await categoryRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.categories.notfound"], id));
    }
}

public class UpdateCourseRequestHandler(
    IRepository<Course> repository,
    IStringLocalizer<UpdateCourseRequestHandler> localizer,
    IFileStorageService file)
    : IRequestHandler<UpdateCourseRequest, Guid>
{
    public async Task<Guid> Handle(UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = course ?? throw new NotFoundException(string.Format(localizer["catalog.courses.update.notfound"], request.Id));

        if (request.DeleteCurrentImage)
        {
            var currentCourseImagePath = course.Image;
            if (!string.IsNullOrEmpty(currentCourseImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                file.Remove(Path.Combine(root, currentCourseImagePath));
            }

            course = course.ClearImagePath();
        }

        var courseImagePath = request.Image is not null
            ? await file.UploadAsync<Course>(request.Image, FileType.Image, cancellationToken)
            : null;

        var updatedCourse = course.Update(request.Title, request.Intrudiction, request.Description, request.CategoryId, courseImagePath, request.Tags, request.Published, request.Upcoming, request.Featured, request.PreviewVideoUrl, request.Paid, request.CompletetionCertificate, request.PaidCertificate);

        course.DomainEvents.Add(EntityUpdatedEvent.WithEntity(course));

        await repository.UpdateAsync(updatedCourse, cancellationToken);

        return request.Id;
    }
}
