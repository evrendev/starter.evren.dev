using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.FileStorage;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Update;

public class UpdateLessonRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Notes { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
    public FileUploadRequest? Image { get; set; }
}

public class UpdateLessonRequestValidator : CustomValidator<UpdateLessonRequest>
{
    public UpdateLessonRequestValidator(IReadRepository<Lesson> lessonRepo, IReadRepository<Chapter> chapterRepo, IStringLocalizer<UpdateLessonRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (lesson, name, ct) =>
                    await lessonRepo.FirstOrDefaultAsync(new LessonByTitleSpec(name), ct)
                        is not Lesson existingLesson || existingLesson.Id == lesson.Id)
                .WithMessage((_, name) => string.Format(localizer["catalog.lessons.update.alreadyexists"], name));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());

        RuleFor(p => p.ChapterId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await chapterRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.chapters.notfound"], id));
    }
}

public class UpdateLessonRequestHandler(
    IRepository<Lesson> repository,
    IStringLocalizer<UpdateLessonRequestHandler> localizer,
    IFileStorageService file)
    : IRequestHandler<UpdateLessonRequest, Guid>
{
    public async Task<Guid> Handle(UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        var lesson = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = lesson ?? throw new NotFoundException(string.Format(localizer["catalog.lessons.update.notfound"], request.Id));

        if (request.DeleteCurrentImage)
        {
            var currentLessonImagePath = lesson.Image;
            if (!string.IsNullOrEmpty(currentLessonImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                file.Remove(Path.Combine(root, currentLessonImagePath));
            }

            lesson = lesson.ClearImagePath();
        }

        var lessonImagePath = request.Image is not null
            ? await file.UploadAsync<Lesson>(request.Image, FileType.Image, cancellationToken)
            : null;

        var updatedLesson = lesson.Update(request.Title, request.Description, request.Content, request.Notes, request.ChapterId, lessonImagePath);

        lesson.DomainEvents.Add(EntityUpdatedEvent.WithEntity(lesson));

        await repository.UpdateAsync(updatedLesson, cancellationToken);

        return request.Id;
    }
}
