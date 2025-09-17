using EvrenDev.Application.Catalog.Courses.Specifications;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Courses.Queries.Create;

public class CreateCourseRequest : IRequest<Guid>
{
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
    public FileUploadRequest? Image { get; set; }
}

public class CreateCourseRequestValidator : CustomValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator(IReadRepository<Course> courseRepo, IReadRepository<Category> categoryRepo, IStringLocalizer<CreateCourseRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await courseRepo.FirstOrDefaultAsync(new CourseByTitleSpec(title), ct) is null)
                .WithMessage((_, title) => string.Format(localizer["catalog.courses.create.alreadyexists"], title));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());

        RuleFor(p => p.CategoryId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await categoryRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.categories.notfound"], id));
    }
}

public class CreateCourseRequestHandler(IRepository<Course> repository, IFileStorageService file) : IRequestHandler<CreateCourseRequest, Guid>
{
    public async Task<Guid> Handle(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var courseImagePath = await file.UploadAsync<Course>(request.Image, FileType.Image, cancellationToken);

        var course = new Course(request.Title, request.Intrudiction, request.Description, request.CategoryId, courseImagePath, request.Tags, request.Published, request.Upcoming, request.Featured, request.PreviewVideoUrl, request.Paid, request.CompletetionCertificate, request.PaidCertificate);

        course.DomainEvents.Add(EntityCreatedEvent.WithEntity(course));

        await repository.AddAsync(course, cancellationToken);

        return course.Id;
    }
}
