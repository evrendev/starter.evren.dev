using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Common.Exporters;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Export;

public class ExportLessonsRequest : BaseFilter, IRequest<Stream>
{
    public Guid? ChapterId { get; set; }
}

public class ExportLessonsWithChaptersSpecification : EntitiesByBaseFilterSpec<Lesson, LessonExportDto>
{
    public ExportLessonsWithChaptersSpecification(ExportLessonsRequest request)
        : base(request) =>
        Query
            .Include(p => p.Chapter)
            .Where(p => p.ChapterId.Equals(request.ChapterId!.Value), request.ChapterId.HasValue);
}

public class ExportLessonsRequestHandler(IReadRepository<Lesson> repository, IExcelWriter excelWriter)
    : IRequestHandler<ExportLessonsRequest, Stream>
{
    public async Task<Stream> Handle(ExportLessonsRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportLessonsWithChaptersSpecification(request);

        var list = await repository.ListAsync(spec, cancellationToken);

        return excelWriter.WriteToStream(list);
    }
}
