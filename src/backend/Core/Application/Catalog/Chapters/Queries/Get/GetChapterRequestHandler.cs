using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Catalog.Chapters.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Get;

public class GetChapterRequest(Guid id) : IRequest<ChapterDto>
{
    public Guid Id { get; set; } = id;
}

public class GetChapterRequestHandler(IRepository<Chapter> repository, IStringLocalizer<GetChapterRequestHandler> localizer)
    : IRequestHandler<GetChapterRequest, ChapterDto>
{
    public async Task<ChapterDto> Handle(GetChapterRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new ChapterByIdWithCategorySpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["catalog.chapters.get.notfound"], request.Id));
}
