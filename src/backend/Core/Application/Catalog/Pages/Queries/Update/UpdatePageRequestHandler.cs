using System.Text.Json.Serialization;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Enums;

namespace EvrenDev.Application.Catalog.Pages.Queries.Update;

public class UpdatePageRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    // The admin page form (frontend PageContentType) sends the enum name
    // (e.g. "Text"), not its numeric value — scoped to this property only, not a global
    // JsonStringEnumConverter, so it doesn't change ImportJobDto.Status's numeric wire
    // format that the PPTX import UI already relies on (see PPTX import Task C/D).
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PageContentType? ContentType { get; set; }
    public int? Order { get; set; }
    public string? MediaUrl { get; set; }
    public bool? IsImported { get; set; }
    // Image/Video content types: an uploaded file, same embedded-upload pattern as
    // Course.Image (see CreateCourseRequestHandler) — takes priority over MediaUrl
    // when present, so the admin never types a storage path/URL by hand.
    public FileUploadRequest? MediaFile { get; set; }
}

public class UpdatePageRequestValidator : CustomValidator<UpdatePageRequest>
{
    public UpdatePageRequestValidator(IReadRepository<Page> pageRepo, IReadRepository<Chapter> chapterRepo, IStringLocalizer<UpdatePageRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (page, name, ct) =>
                    await pageRepo.FirstOrDefaultAsync(new PageByTitleSpec(name), ct)
                        is not Page existingPage || existingPage.Id == page.Id)
                .WithMessage((_, name) => string.Format(localizer["catalog.pages.update.alreadyexists"], name));

        RuleFor(p => p.ChapterId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await chapterRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.chapters.notfound"], id));

        RuleFor(p => p.Content)
            .NotEmpty();

        RuleFor(p => p.MediaFile)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}

public class UpdatePageRequestHandler(
    IRepository<Page> repository,
    IFileStorageService file,
    IStringLocalizer<UpdatePageRequestHandler> localizer)
    : IRequestHandler<UpdatePageRequest, Guid>
{
    public async Task<Guid> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
    {
        var page = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = page ?? throw new NotFoundException(string.Format(localizer["catalog.pages.update.notfound"], request.Id));

        var mediaUrl = request.MediaUrl;
        if (request.MediaFile is not null)
        {
            var fileType = request.ContentType == PageContentType.Video ? FileType.Video : FileType.Image;
            mediaUrl = await file.UploadAsync<Page>(request.MediaFile, fileType, cancellationToken);
        }

        var updatedPage = page.Update(request.Title, request.Content, request.ContentType,
            request.Order, mediaUrl, isImported: request.IsImported);

        await repository.UpdateAsync(updatedPage, cancellationToken);

        return request.Id;
    }
}
