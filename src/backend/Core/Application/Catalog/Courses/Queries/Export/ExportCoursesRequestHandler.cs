using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Common.Exporters;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Queries.Export;

public class ExportCoursesRequest : BaseFilter, IRequest<Stream>
{
    public Guid? CategoryId { get; set; }
}

public class ExportCoursesWithCategoriesSpecification : EntitiesByBaseFilterSpec<Course, CourseExportDto>
{
    public ExportCoursesWithCategoriesSpecification(ExportCoursesRequest request)
        : base(request) =>
        Query
            .Include(p => p.Category)
            .Where(p => p.CategoryId.Equals(request.CategoryId!.Value), request.CategoryId.HasValue);
}

public class ExportCoursesRequestHandler(IReadRepository<Course> repository, IExcelWriter excelWriter)
    : IRequestHandler<ExportCoursesRequest, Stream>
{
    public async Task<Stream> Handle(ExportCoursesRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportCoursesWithCategoriesSpecification(request);

        var list = await repository.ListAsync(spec, cancellationToken);

        return excelWriter.WriteToStream(list);
    }
}
