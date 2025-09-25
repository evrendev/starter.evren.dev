using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Catalog.Lessons.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Get;

public class GetLessonRequest(Guid id) : IRequest<LessonDetailsDto>
{
    public Guid Id { get; set; } = id;
}

public class GetLessonRequestHandler(IRepository<Lesson> repository, IStringLocalizer<GetLessonRequestHandler> localizer)
    : IRequestHandler<GetLessonRequest, LessonDetailsDto>
{
    public async Task<LessonDetailsDto> Handle(GetLessonRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new LessonByIdWithChapterSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["catalog.lessons.get.notfound"], request.Id));
}
