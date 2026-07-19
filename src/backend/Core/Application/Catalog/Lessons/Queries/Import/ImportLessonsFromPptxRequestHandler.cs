using System.Net;
using System.Text.Json;
using EvrenDev.Application.Catalog.Lessons.Interfaces;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Import;

public class ImportLessonsFromPptxRequest : IRequest<Guid>
{
    public Guid CourseId { get; set; }
    public IFormFile File { get; set; } = default!;
    // Raw JSON array of per-slide HTML from the client-side pptx-to-html render
    // (@jvmr/pptx-to-html), one string per slide in slide order. Optional: when null,
    // missing, or unparseable, the job falls back to its own OpenXml text extraction —
    // this must never fail the import (see PPTX import Task F).
    public string? SlidesHtmlJson { get; set; }
}

public class ImportLessonsFromPptxRequestValidator : CustomValidator<ImportLessonsFromPptxRequest>
{
    public ImportLessonsFromPptxRequestValidator(IReadRepository<Course> courseRepo,
        IStringLocalizer<ImportLessonsFromPptxRequestValidator> localizer)
    {
        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));

        RuleFor(p => p.File)
            .NotNull()
            .Must(f => string.Equals(Path.GetExtension(f.FileName), ".pptx", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only .pptx files are supported.")
            .Must(f => f.Length is > 0 and <= ImportLessonsFromPptxRequestHandler.MaxFileSizeBytes)
                .WithMessage(
                    $"File size must be between 1 byte and {ImportLessonsFromPptxRequestHandler.MaxFileSizeBytes / (1024 * 1024)}MB.");
    }
}

// NOTE: this validator, like every FluentValidation validator in this codebase, is
// registered in DI (services.AddValidatorsFromAssembly) but there is no MediatR
// IPipelineBehavior<,> anywhere that actually invokes IValidator<T> before a handler
// runs — confirmed by searching the whole backend. It does not run automatically.
// The handler below re-checks the same rules directly so this endpoint is actually
// enforced; the validator is kept for documentation/consistency with sibling requests.
public class ImportLessonsFromPptxRequestHandler(
    IReadRepository<Course> courseRepository,
    IRepository<ImportJob> importJobRepository,
    IFileStorageService fileStorageService,
    IJobService jobService,
    ICurrentUser currentUser,
    ILogger<ImportLessonsFromPptxRequestHandler> logger) : IRequestHandler<ImportLessonsFromPptxRequest, Guid>
{
    public const long MaxFileSizeBytes = 150 * 1024 * 1024;

    public async Task<Guid> Handle(ImportLessonsFromPptxRequest request, CancellationToken cancellationToken)
    {
        if (await courseRepository.GetByIdAsync(request.CourseId, cancellationToken) is null)
            throw new NotFoundException($"Course with ID '{request.CourseId}' not found.");

        if (!string.Equals(Path.GetExtension(request.File.FileName), ".pptx", StringComparison.OrdinalIgnoreCase))
            throw new CustomException("Only .pptx files are supported.", statusCode: HttpStatusCode.BadRequest);

        if (request.File.Length is 0 or > MaxFileSizeBytes)
            throw new CustomException(
                $"File size must be between 1 byte and {MaxFileSizeBytes / (1024 * 1024)}MB.",
                statusCode: HttpStatusCode.BadRequest);

        await using var stream = request.File.OpenReadStream();
        var filePath = await fileStorageService.SaveTempFileAsync(stream, request.File.FileName, cancellationToken);

        var userId = currentUser.GetUserId().ToString();

        // TotalSlides is unknown here on purpose: counting it would require opening the
        // .pptx with DocumentFormat.OpenXml, which is an Infrastructure-only dependency
        // (see docs/backend-stack.md decision in Task B). The job fills in the real count
        // the moment it opens the file, before the first progress update.
        var importJob = new ImportJob(request.CourseId);
        await importJobRepository.AddAsync(importJob, cancellationToken);

        var slidesHtml = TryParseSlidesHtml(request.SlidesHtmlJson);

        // `default` for the token, not `cancellationToken`: the HTTP request (and its
        // token) will already be gone by the time Hangfire actually runs this job — same
        // convention as the existing IBrandGeneratorJob.GenerateAsync(...) enqueue call.
        jobService.Enqueue<IImportLessonsFromPptxJob>(
            x => x.ExecuteAsync(importJob.Id, request.CourseId, filePath, userId, slidesHtml, default));

        return importJob.Id;
    }

    private List<string>? TryParseSlidesHtml(string? slidesHtmlJson)
    {
        if (string.IsNullOrWhiteSpace(slidesHtmlJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<List<string>>(slidesHtmlJson);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Could not parse client-provided slidesHtml payload; falling back to server-side extraction");
            return null;
        }
    }
}
