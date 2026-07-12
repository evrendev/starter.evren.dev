using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Get;

public class LessonPlayerPageDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int Order { get; set; }
    public string ContentType { get; set; } = default!;
}

public class GetLessonPlayerRequest(Guid lessonId) : IRequest<LessonPlayerDto>
{
    public Guid LessonId { get; set; } = lessonId;
}

public class LessonPlayerDto
{
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = default!;
    public List<LessonPlayerPageDto> Pages { get; set; } = [];
}

public class GetLessonPlayerRequestHandler(IRepository<Lesson> lessonRepository)
    : IRequestHandler<GetLessonPlayerRequest, LessonPlayerDto>
{
    public async Task<LessonPlayerDto> Handle(GetLessonPlayerRequest request, CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);
        if (lesson == null)
            throw new NotFoundException($"Lesson with ID '{request.LessonId}' not found.");

        var pages = lesson.Pages?.OrderBy(p => p.Order)
            .Select(p => new LessonPlayerPageDto
            {
                Id = p.Id,
                Title = p.Title,
                Order = p.Order,
                ContentType = p.ContentType.ToString()
            })
            .ToList() ?? [];

        return new LessonPlayerDto
        {
            LessonId = lesson.Id,
            LessonTitle = lesson.Title,
            Pages = pages
        };
    }
}
