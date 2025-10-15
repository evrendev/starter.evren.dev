using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Update;

public class UpdateLessonRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = default!;
    public string? Content { get; set; }
}

public class UpdateLessonRequestValidator : CustomValidator<UpdateLessonRequest>
{
    public UpdateLessonRequestValidator(IReadRepository<Lesson> lessonRepo, IReadRepository<Chapter> chapterRepo, IStringLocalizer<UpdateLessonRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (lesson, name, ct) =>
                    await lessonRepo.FirstOrDefaultAsync(new LessonByTitleSpec(name), ct)
                        is not Lesson existingLesson || existingLesson.Id == lesson.Id)
                .WithMessage((_, name) => string.Format(localizer["catalog.lessons.update.alreadyexists"], name));

        RuleFor(p => p.ChapterId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await chapterRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.chapters.notfound"], id));
    }
}

public class UpdateLessonRequestHandler(
    IRepository<Lesson> repository,
    IStringLocalizer<UpdateLessonRequestHandler> localizer)
    : IRequestHandler<UpdateLessonRequest, Guid>
{
    public async Task<Guid> Handle(UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        var lesson = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = lesson ?? throw new NotFoundException(string.Format(localizer["catalog.lessons.update.notfound"], request.Id));

        var updatedLesson = lesson.Update(request.Title, request.Content, request.ChapterId);

        lesson.DomainEvents.Add(EntityUpdatedEvent.WithEntity(lesson));

        await repository.UpdateAsync(updatedLesson, cancellationToken);

        return request.Id;
    }
}
