using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Common.Exporters;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Export;

public class ExportChaptersRequest : BaseFilter, IRequest<Stream>
{
    public Guid? CourseId { get; set; }
}

public class ExportChaptersWithCoursesSpecification : EntitiesByBaseFilterSpec<Chapter, ChapterDto>
{
    public ExportChaptersWithCoursesSpecification(ExportChaptersRequest request)
        : base(request) =>
        Query
            .Include(p => p.Course)
            .Where(p => p.CourseId.Equals(request.CourseId!.Value), request.CourseId.HasValue);
}

public class ExportChaptersRequestHandler(IReadRepository<Chapter> repository, IExcelWriter excelWriter)
    : IRequestHandler<ExportChaptersRequest, Stream>
{
    public async Task<Stream> Handle(ExportChaptersRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportChaptersWithCoursesSpecification(request);

        var list = await repository.ListAsync(spec, cancellationToken);

        return excelWriter.WriteToStream(list);
    }
}
