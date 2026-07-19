using System.Text.Json.Serialization;
using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Create;

public class CreateLessonPageRequest : IRequest<Guid>
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    // Same admin form sends this for both Create and Update - see the matching note
    // on UpdateLessonPageRequest.ContentType
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LessonPageContentType ContentType { get; set; }
    public int Order { get; set; } = 0;
    public string? MediaUrl { get; set; }
}

public class CreateLessonPageRequestValidator : CustomValidator<CreateLessonPageRequest>
{
    public CreateLessonPageRequestValidator(IReadRepository<LessonPage> lessonPageRepo, IReadRepository<Lesson> lessonRepo, IStringLocalizer<CreateLessonPageRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await lessonPageRepo.FirstOrDefaultAsync(new LessonPageByTitleSpec(title), ct) is null)
                .WithMessage((_, title) => string.Format(localizer["catalog.lessonpages.create.alreadyexists"], title));

        RuleFor(p => p.LessonId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await lessonRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.lessons.notfound"], id));

        RuleFor(p => p.Content)
            .NotEmpty();

        RuleFor(p => p.ContentType)
            .IsInEnum();
    }
}

public class CreateLessonPageRequestHandler(IRepository<LessonPage> repository) : IRequestHandler<CreateLessonPageRequest, Guid>
{
    public async Task<Guid> Handle(CreateLessonPageRequest request, CancellationToken cancellationToken)
    {
        var lessonPage = new LessonPage(request.Title, request.Content, request.ContentType,
            request.Order, request.LessonId, request.MediaUrl);

        await repository.AddAsync(lessonPage, cancellationToken);

        return lessonPage.Id;
    }
}
