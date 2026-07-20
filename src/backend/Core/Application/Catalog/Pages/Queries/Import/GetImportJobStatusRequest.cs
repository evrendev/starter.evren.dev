using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Import;

public class GetImportJobStatusRequest(Guid importJobId) : IRequest<ImportJobDto>
{
    public Guid ImportJobId { get; set; } = importJobId;
}

public class GetImportJobStatusRequestHandler(IReadRepository<ImportJob> repository)
    : IRequestHandler<GetImportJobStatusRequest, ImportJobDto>
{
    public async Task<ImportJobDto> Handle(GetImportJobStatusRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(new ImportJobByIdSpec(request.ImportJobId), cancellationToken)
        ?? throw new NotFoundException($"Import job with ID '{request.ImportJobId}' not found.");
}
