using System.Text.Json.Serialization;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Enums;

namespace EvrenDev.Application.Catalog.Pages.Queries.Create;

public class OptionRequest
{
    public string Label { get; set; } = default!;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}

public class QuestionRequest
{
    public string Prompt { get; set; } = default!;
    public int Order { get; set; }
    public List<OptionRequest> Options { get; set; } = [];
}

public class CreatePageRequest : IRequest<Guid>
{
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    // Same admin form sends this for both Create and Update - see the matching note
    // on UpdatePageRequest.ContentType
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PageContentType ContentType { get; set; }
    public int Order { get; set; } = 0;
    public string? MediaUrl { get; set; }
    // Image/Video content types: an uploaded file, same embedded-upload pattern as
    // Course.Image (see CreateCourseRequestHandler) — takes priority over MediaUrl
    // when present, so the admin never types a storage path/URL by hand.
    public FileUploadRequest? MediaFile { get; set; }
    // Quiz content type: structural questions, embedded replace-all in the same
    // request as the rest of the page (no separate question CRUD endpoints — see
    // Task N0/N1). Null means "no questions on create", not "leave untouched" —
    // there's nothing to leave untouched yet on a brand-new page.
    public List<QuestionRequest>? Questions { get; set; }
}

public class CreatePageRequestValidator : CustomValidator<CreatePageRequest>
{
    public CreatePageRequestValidator(IReadRepository<Page> pageRepo, IReadRepository<Chapter> chapterRepo, IStringLocalizer<CreatePageRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await pageRepo.FirstOrDefaultAsync(new PageByTitleSpec(title), ct) is null)
                .WithMessage((_, title) => string.Format(localizer["catalog.pages.create.alreadyexists"], title));

        RuleFor(p => p.ChapterId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await chapterRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.chapters.notfound"], id));

        RuleFor(p => p.Content)
            .NotEmpty();

        RuleFor(p => p.ContentType)
            .IsInEnum();

        RuleFor(p => p.MediaFile)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}

public class CreatePageRequestHandler(IRepository<Page> repository, IFileStorageService file) : IRequestHandler<CreatePageRequest, Guid>
{
    public async Task<Guid> Handle(CreatePageRequest request, CancellationToken cancellationToken)
    {
        var mediaUrl = request.MediaUrl;
        if (request.MediaFile is not null)
        {
            var fileType = request.ContentType == PageContentType.Video ? FileType.Video : FileType.Image;
            mediaUrl = await file.UploadAsync<Page>(request.MediaFile, fileType, cancellationToken);
        }

        var page = new Page(request.Title, request.Content, request.ContentType,
            request.Order, request.ChapterId, mediaUrl);

        if (request.Questions is not null)
        {
            page.ReplaceQuestions(request.Questions.Select(q =>
                new QuestionData(q.Prompt, q.Order, q.Options.Select(o =>
                    new OptionData(o.Label, o.IsCorrect, o.Order)))));
        }

        await repository.AddAsync(page, cancellationToken);

        return page.Id;
    }
}
