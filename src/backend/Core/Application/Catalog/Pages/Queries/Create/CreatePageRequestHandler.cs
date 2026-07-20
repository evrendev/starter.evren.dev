using System.Text.Json.Serialization;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Create;

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
    }
}

public class CreatePageRequestHandler(IRepository<Page> repository) : IRequestHandler<CreatePageRequest, Guid>
{
    public async Task<Guid> Handle(CreatePageRequest request, CancellationToken cancellationToken)
    {
        var page = new Page(request.Title, request.Content, request.ContentType,
            request.Order, request.ChapterId, request.MediaUrl);

        await repository.AddAsync(page, cancellationToken);

        return page.Id;
    }
}
