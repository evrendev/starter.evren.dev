using System.Net;
using EvrenDev.Application.Catalog.Lessons.Interfaces;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using Microsoft.AspNetCore.Http;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Import;

public class ImportLessonsFromPptxRequest : IRequest<string>
{
    public Guid CourseId { get; set; }
    public IFormFile File { get; set; } = default!;
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
    IFileStorageService fileStorageService,
    IJobService jobService,
    ICurrentUser currentUser) : IRequestHandler<ImportLessonsFromPptxRequest, string>
{
    public const long MaxFileSizeBytes = 150 * 1024 * 1024;

    public async Task<string> Handle(ImportLessonsFromPptxRequest request, CancellationToken cancellationToken)
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

        // `default` for the token, not `cancellationToken`: the HTTP request (and its
        // token) will already be gone by the time Hangfire actually runs this job — same
        // convention as the existing IBrandGeneratorJob.GenerateAsync(...) enqueue call.
        var jobId = jobService.Enqueue<IImportLessonsFromPptxJob>(
            x => x.ExecuteAsync(request.CourseId, filePath, userId, default));

        return jobId;
    }
}
