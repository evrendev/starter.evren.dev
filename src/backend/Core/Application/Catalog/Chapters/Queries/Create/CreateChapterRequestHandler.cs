using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Create;

public class CreateChapterRequest : IRequest<Guid>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateChapterRequestValidator : CustomValidator<CreateChapterRequest>
{
    public CreateChapterRequestValidator(IReadRepository<Chapter> chapterRepo, IReadRepository<Course> courseRepo, IStringLocalizer<CreateChapterRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await chapterRepo.FirstOrDefaultAsync(new ChapterByTitleSpec(title), ct) is null)
                .WithMessage((_, title) => string.Format(localizer["catalog.chapters.create.alreadyexists"], title));

        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));
    }
}

public class CreateChapterRequestHandler(IRepository<Chapter> repository) : IRequestHandler<CreateChapterRequest, Guid>
{
    public async Task<Guid> Handle(CreateChapterRequest request, CancellationToken cancellationToken)
    {
        var chapter = new Chapter(request.Title, request.Description, request.CourseId);

        chapter.DomainEvents.Add(EntityCreatedEvent.WithEntity(chapter));

        await repository.AddAsync(chapter, cancellationToken);

        return chapter.Id;
    }
}
