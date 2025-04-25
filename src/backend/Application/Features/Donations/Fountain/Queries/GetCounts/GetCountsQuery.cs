using EvrenDev.Application.Features.Donations.Fountain.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Queries.GetCounts;

public class GetCountsQuery : IRequest<Result<List<ProjectsCountDto>?>>
{
    public string? ProjectCode { get; init; }
    public DateTime? StartDate { get; init; } = new DateTime(DateTime.Now.Year, 1, 1);
    public DateTime? EndDate { get; init; } = new DateTime(DateTime.Now.Year, 12, 31);
}

public class GetCountsQueryValidator : AbstractValidator<GetCountsQuery>
{
    private readonly IStringLocalizer<GetCountsQueryValidator> _localizer;

    public GetCountsQueryValidator(IStringLocalizer<GetCountsQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.StartDate)
            .LessThan(v => v.EndDate)
            .When(v => v.StartDate.HasValue && v.EndDate.HasValue)
            .WithMessage(_localizer["api.donations.fountains.startdate.less.than.enddate"]);

        RuleFor(v => v.EndDate)
            .GreaterThan(v => v.StartDate)
            .When(v => v.StartDate.HasValue && v.EndDate.HasValue)
            .WithMessage(_localizer["api.donations.fountains.enddate.greater.than.startdate"]);
    }
}

public class GetCountsQueryHandler : IRequestHandler<GetCountsQuery, Result<List<ProjectsCountDto>?>>
{
    private readonly IDonationDbContext _context;

    public GetCountsQueryHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ProjectsCountDto>?>> Handle(GetCountsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FountainDonations.AsQueryable();

        if (request.StartDate != null)
            query = query.Where(entity => entity.CreationDate >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.CreationDate <= request.EndDate);

        if (!string.IsNullOrEmpty(request.ProjectCode))
            query = query.Where(entity => entity.ProjectCode == request.ProjectCode);

        var counts = await query
            .GroupBy(x => x.ProjectCode)
            .Select(g => new ProjectsCountDto
            {
                Project = FountainDonationProject.FromName(g.Key),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result<List<ProjectsCountDto>?>.Success(counts);
    }
}
