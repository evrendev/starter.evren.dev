using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Update;

public class UpdateChapterRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}

public class UpdateChapterRequestValidator : CustomValidator<UpdateChapterRequest>
{
    public UpdateChapterRequestValidator(IReadRepository<Chapter> chapterRepo, IReadRepository<Course> courseRepo, IStringLocalizer<UpdateChapterRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (chapter, name, ct) =>
                    await chapterRepo.FirstOrDefaultAsync(new ChapterByTitleSpec(name), ct)
                        is not Chapter existingChapter || existingChapter.Id == chapter.Id)
                .WithMessage((_, name) => string.Format(localizer["catalog.chapters.update.alreadyexists"], name));

        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));
    }
}

public class UpdateChapterRequestHandler(
    IRepository<Chapter> repository,
    IStringLocalizer<UpdateChapterRequestHandler> localizer)
    : IRequestHandler<UpdateChapterRequest, Guid>
{
    public async Task<Guid> Handle(UpdateChapterRequest request, CancellationToken cancellationToken)
    {
        var chapter = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = chapter ?? throw new NotFoundException(string.Format(localizer["catalog.chapters.update.notfound"], request.Id));
        var updatedChapter = chapter.Update(request.Title, request.Description, request.CourseId);

        chapter.DomainEvents.Add(EntityUpdatedEvent.WithEntity(chapter));

        await repository.UpdateAsync(updatedChapter, cancellationToken);

        return request.Id;
    }
}
