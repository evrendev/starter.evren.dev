using System.Text.Json.Serialization;
using EvrenDev.Application.Catalog.LessonPages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Update;

public class UpdateLessonPageRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    // The admin lesson-page form (frontend LessonPageContentType) sends the enum name
    // (e.g. "Text"), not its numeric value — scoped to this property only, not a global
    // JsonStringEnumConverter, so it doesn't change ImportJobDto.Status's numeric wire
    // format that the PPTX import UI already relies on (see PPTX import Task C/D).
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LessonPageContentType? ContentType { get; set; }
    public int? Order { get; set; }
    public string? MediaUrl { get; set; }
}

public class UpdateLessonPageRequestValidator : CustomValidator<UpdateLessonPageRequest>
{
    public UpdateLessonPageRequestValidator(IReadRepository<LessonPage> lessonPageRepo, IReadRepository<Lesson> lessonRepo, IStringLocalizer<UpdateLessonPageRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (lessonPage, name, ct) =>
                    await lessonPageRepo.FirstOrDefaultAsync(new LessonPageByTitleSpec(name), ct)
                        is not LessonPage existingPage || existingPage.Id == lessonPage.Id)
                .WithMessage((_, name) => string.Format(localizer["catalog.lessonpages.update.alreadyexists"], name));

        RuleFor(p => p.LessonId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await lessonRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.lessons.notfound"], id));

        RuleFor(p => p.Content)
            .NotEmpty();
    }
}

public class UpdateLessonPageRequestHandler(
    IRepository<LessonPage> repository,
    IStringLocalizer<UpdateLessonPageRequestHandler> localizer)
    : IRequestHandler<UpdateLessonPageRequest, Guid>
{
    public async Task<Guid> Handle(UpdateLessonPageRequest request, CancellationToken cancellationToken)
    {
        var lessonPage = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = lessonPage ?? throw new NotFoundException(string.Format(localizer["catalog.lessonpages.update.notfound"], request.Id));

        var updatedPage = lessonPage.Update(request.Title, request.Content, request.ContentType,
            request.Order, request.MediaUrl);

        await repository.UpdateAsync(updatedPage, cancellationToken);

        return request.Id;
    }
}
