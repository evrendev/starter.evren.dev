using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using Mapster;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Get;

public class GetAllChaptersRequest() : IRequest<List<ChapterDto>>
{
}

public class GetAllChaptersRequestHandler(IRepository<Chapter> repository, IStringLocalizer<GetAllChaptersRequestHandler> localizer)
    : IRequestHandler<GetAllChaptersRequest, List<ChapterDto>>
{
    public async Task<List<ChapterDto>> Handle(GetAllChaptersRequest request, CancellationToken cancellationToken)
    {
        var chapters = await repository.ListAsync(cancellationToken);

        if (chapters == null || !chapters.Any())
            throw new NotFoundException(string.Format(localizer["catalog.chapters.list.notfound"]));

        return chapters.Adapt<List<ChapterDto>>();
    }
}
