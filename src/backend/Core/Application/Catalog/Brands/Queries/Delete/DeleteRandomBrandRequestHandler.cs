using EvrenDev.Application.Catalog.Brands.Interfaces;

namespace EvrenDev.Application.Catalog.Brands.Queries.Delete;

public class DeleteRandomBrandRequest : IRequest<string>
{
}

public class DeleteRandomBrandRequestHandler(IJobService jobService) : IRequestHandler<DeleteRandomBrandRequest, string>
{
    public Task<string> Handle(DeleteRandomBrandRequest request, CancellationToken cancellationToken)
    {
        var jobId = jobService.Schedule<IBrandGeneratorJob>(x => x.CleanAsync(default), TimeSpan.FromSeconds(5));
        return Task.FromResult(jobId);
    }
}
