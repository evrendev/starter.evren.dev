using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Create;

public class CreateLessonRequest : IRequest<Guid>
{
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Notes { get; set; }
    public FileUploadRequest? Image { get; set; }
}

public class CreateLessonRequestValidator : CustomValidator<CreateLessonRequest>
{
    public CreateLessonRequestValidator(IReadRepository<Lesson> lessonRepo, IReadRepository<Chapter> chapterRepo, IStringLocalizer<CreateLessonRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await lessonRepo.FirstOrDefaultAsync(new LessonByTitleSpec(title), ct) is null)
                .WithMessage((_, title) => string.Format(localizer["catalog.lessons.create.alreadyexists"], title));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());

        RuleFor(p => p.ChapterId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await chapterRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.chapters.notfound"], id));
    }
}

public class CreateLessonRequestHandler(IRepository<Lesson> repository, IFileStorageService file) : IRequestHandler<CreateLessonRequest, Guid>
{
    public async Task<Guid> Handle(CreateLessonRequest request, CancellationToken cancellationToken)
    {
        var lessonImagePath = await file.UploadAsync<Lesson>(request.Image, FileType.Image, cancellationToken);

        var lesson = new Lesson(request.Title, request.Description, request.Content, request.Notes, request.ChapterId, lessonImagePath);

        lesson.DomainEvents.Add(EntityCreatedEvent.WithEntity(lesson));

        await repository.AddAsync(lesson, cancellationToken);

        return lesson.Id;
    }
}
