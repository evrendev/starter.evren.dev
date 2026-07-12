using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Common.Exporters;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Queries.Export;

public class ExportLessonPagesRequest : BaseFilter, IRequest<Stream>
{
    public Guid? LessonId { get; set; }
}

public class ExportLessonPagesWithLessonsSpecification : EntitiesByBaseFilterSpec<LessonPage, LessonPageExportDto>
{
    public ExportLessonPagesWithLessonsSpecification(ExportLessonPagesRequest request)
        : base(request) =>
        Query
            .Include(p => p.Lesson)
            .Where(p => p.LessonId.Equals(request.LessonId!.Value), request.LessonId.HasValue)
            .OrderBy(p => p.Order);
}

public class ExportLessonPagesRequestHandler(IReadRepository<LessonPage> repository, IExcelWriter excelWriter)
    : IRequestHandler<ExportLessonPagesRequest, Stream>
{
    public async Task<Stream> Handle(ExportLessonPagesRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportLessonPagesWithLessonsSpecification(request);

        var list = await repository.ListAsync(spec, cancellationToken);

        return excelWriter.WriteToStream(list);
    }
}
