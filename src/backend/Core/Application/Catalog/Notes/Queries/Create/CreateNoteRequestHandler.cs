using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Queries.Create;

public class CreateNoteRequest : IRequest<Guid>
{
    public string UserId { get; set; } = default!;
    public Guid PageId { get; set; }
    public string Content { get; set; } = default!;
}

public class CreateNoteRequestValidator : CustomValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator(IReadRepository<Page> pageRepo, IStringLocalizer<CreateNoteRequestValidator> localizer)
    {
        RuleFor(p => p.UserId)
            .NotEmpty()
            .WithMessage((_, id) => string.Format(localizer["shared.userid.required"]));

        RuleFor(p => p.Content)
            .NotEmpty()
            .WithMessage((_, content) => string.Format(localizer["catalog.notes.create.contentrequired"]));

        RuleFor(p => p.PageId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await pageRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.pages.notfound"], id));
    }
}

public class CreateNoteRequestHandler(
    IRepositoryWithEvents<Note> repository,
    IReadRepository<Page> pageRepository,
    IReadRepository<CourseEnrollment> courseEnrollmentRepository,
    ICurrentUser currentUser) : IRequestHandler<CreateNoteRequest, Guid>
{
    public async Task<Guid> Handle(CreateNoteRequest request, CancellationToken cancellationToken)
    {
        var page = await pageRepository.FirstOrDefaultAsync(
            new PageWithChapterSpec(request.PageId), cancellationToken);

        if (page is null)
            throw new NotFoundException($"Page with ID '{request.PageId}' not found.");

        // Same enrollment + staging gate as GetChapterPlayerRequestHandler /
        // MarkPageCompletedRequestHandler (see PPTX import Task H) — a student must
        // not be able to attach a note to a page they couldn't otherwise reach
        var userId = currentUser.GetUserId().ToString();

        var isEnrolled = await courseEnrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, page.Chapter.CourseId), cancellationToken) is not null;

        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in the course this page belongs to.");

        if (page.Chapter.IsStaging)
            throw new ForbiddenException("This content has not been published yet.");

        var note = new Note(request.UserId, request.PageId, request.Content);

        await repository.AddAsync(note, cancellationToken);

        return note.Id;
    }
}
