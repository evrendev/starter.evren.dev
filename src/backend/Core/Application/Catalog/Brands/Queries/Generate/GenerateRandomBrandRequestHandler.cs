using EvrenDev.Application.Catalog.Brands.Interfaces;

namespace EvrenDev.Application.Catalog.Brands.Queries.Generate;

public class GenerateRandomBrandRequest : IRequest<string>
{
    public int NSeed { get; set; }
}

public class GenerateRandomBrandRequestHandler(IJobService jobService) : IRequestHandler<GenerateRandomBrandRequest, string>
{
    public Task<string> Handle(GenerateRandomBrandRequest request, CancellationToken cancellationToken)
    {
        var jobId = jobService.Enqueue<IBrandGeneratorJob>(x => x.GenerateAsync(request.NSeed, default));
        return Task.FromResult(jobId);
    }
}
