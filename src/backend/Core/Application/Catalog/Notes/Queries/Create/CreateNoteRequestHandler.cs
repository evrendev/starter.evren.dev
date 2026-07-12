using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Queries.Create;

public class CreateNoteRequest : IRequest<Guid>
{
    public string UserId { get; set; } = default!;
    public Guid LessonPageId { get; set; }
    public string Content { get; set; } = default!;
}

public class CreateNoteRequestValidator : CustomValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator(IReadRepository<LessonPage> lessonPageRepo, IStringLocalizer<CreateNoteRequestValidator> localizer)
    {
        RuleFor(p => p.UserId)
            .NotEmpty()
            .WithMessage((_, id) => string.Format(localizer["shared.userid.required"]));

        RuleFor(p => p.Content)
            .NotEmpty()
            .WithMessage((_, content) => string.Format(localizer["catalog.notes.create.contentrequired"]));

        RuleFor(p => p.LessonPageId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await lessonPageRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.lessonpages.notfound"], id));
    }
}

public class CreateNoteRequestHandler(IRepository<Note> repository) : IRequestHandler<CreateNoteRequest, Guid>
{
    public async Task<Guid> Handle(CreateNoteRequest request, CancellationToken cancellationToken)
    {
        var note = new Note(request.UserId, request.LessonPageId, request.Content);

        await repository.AddAsync(note, cancellationToken);

        return note.Id;
    }
}
