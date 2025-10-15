using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Create;

public class CreateLessonRequest : IRequest<Guid>
{
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = default!;
    public string? Content { get; set; }
}

public class CreateLessonRequestValidator : CustomValidator<CreateLessonRequest>
{
    public CreateLessonRequestValidator(IReadRepository<Lesson> lessonRepo, IReadRepository<Chapter> chapterRepo, IStringLocalizer<CreateLessonRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await lessonRepo.FirstOrDefaultAsync(new LessonByTitleSpec(title), ct) is null)
                .WithMessage((_, title) => string.Format(localizer["catalog.lessons.create.alreadyexists"], title));

        RuleFor(p => p.ChapterId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await chapterRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.chapters.notfound"], id));
    }
}

public class CreateLessonRequestHandler(IRepository<Lesson> repository) : IRequestHandler<CreateLessonRequest, Guid>
{
    public async Task<Guid> Handle(CreateLessonRequest request, CancellationToken cancellationToken)
    {
        var lesson = new Lesson(request.Title, request.Content, request.ChapterId);

        lesson.DomainEvents.Add(EntityCreatedEvent.WithEntity(lesson));

        await repository.AddAsync(lesson, cancellationToken);

        return lesson.Id;
    }
}
