using System.Globalization;
using EvrenDev.Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Features.Donations.Fountain.Queries.GetDonationsMetrics;

public class GetDonationsMetricsQuery : IRequest<Result<DonationMetrics?>>
{
    public string? Project { get; init; }
    public DateTime? StartDate { get; init; } = new DateTime(DateTime.Now.Year, 1, 1);
    public DateTime? EndDate { get; init; } = new DateTime(DateTime.Now.Year, 12, 31);
}

public class GetDonationsMetricsQueryValidator : AbstractValidator<GetDonationsMetricsQuery>
{
    private readonly IStringLocalizer<GetDonationsMetricsQueryValidator> _localizer;

    public GetDonationsMetricsQueryValidator(IStringLocalizer<GetDonationsMetricsQueryValidator> localizer)
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

public class GetDonationsMetricsQueryHandler : IRequestHandler<GetDonationsMetricsQuery, Result<DonationMetrics?>>
{
    private readonly IDonationDbContext _context;
    private readonly ILogger<GetDonationsMetricsQueryHandler> _logger;

    public GetDonationsMetricsQueryHandler(IDonationDbContext context,
        ILogger<GetDonationsMetricsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<DonationMetrics?>> Handle(GetDonationsMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var query = _context.FountainDonations.AsQueryable().Where(donation => donation.Source != "EMPTY");

            if (request.StartDate != null)
                query = query.Where(entity => entity.CreationDate >= request.StartDate);

            if (request.EndDate != null)
                query = query.Where(entity => entity.CreationDate <= request.EndDate);

            if (!string.IsNullOrEmpty(request.Project))
                query = query.Where(entity => entity.Project == request.Project);

            var totalFountainCountsByProject = await query
                .GroupBy(x => x.Project)
                .OrderBy(x => x.Key)
                .Select(g => new ProjectsCountDto
                {
                    Project = FountainDonationProject.FromName(g.Key),
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var groupedDonations = await query
                .GroupBy(d => d.Project)
                .ToListAsync(cancellationToken);

            var recentFountainDonations = groupedDonations
                .SelectMany(g => g
                    .OrderByDescending(d => d.CreationDate)
                    .Take(3)
                )
                .OrderBy(d => d.Project)
                .Select(entity => new BasicFountainDonationDto
                {
                    Id = entity.Id,
                    Contact = entity.Contact,
                    Phone = Tools.CreatePhone(entity.Phone, $"{entity.Project}-{entity.ProjectNumber}", entity.Banner),
                    CreationDate = DateTimeDto.Create.FromUtc(entity.CreationDate),
                    HtmlBanner = $"<strong>{entity.Project}-{entity.ProjectNumber}:</strong> {entity.Banner}",
                    PlainBanner = $"{entity.Project}-{entity.ProjectNumber}: {entity.Banner}",
                    Team = FountaionTeam.From(entity.Team),
                    MediaStatus = MediaStatus.From(entity.MediaStatus),
                    MediaInformation = entity.MediaInformation,
                })
                .ToList();

            var donationCountByMonth = await query
                .GroupBy(d => new { d.CreationDate.Year, d.CreationDate.Month, d.Project })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Project,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var monthlyProjectStats = donationCountByMonth
                .Select(g => new MonthlyProjectStatsDto
                {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMM", CultureInfo.InvariantCulture),
                    Project = FountainDonationProject.FromName(g.Project),
                    Count = g.Count
                })
                .OrderBy(g => g.Month)
                .ThenBy(g => g.Project?.Name)
                .ToList();

            var response = new DonationMetrics()
            {
                TotalFountainCountsByProject = totalFountainCountsByProject,
                RecentFountainDonations = recentFountainDonations,
                MonthlyProjectStats = monthlyProjectStats
            };

            return Result<DonationMetrics?>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting donations overview");
            return Result<DonationMetrics?>.Failure(new[] { "api.donations.fountains.overview.error" });
            throw;
        }
    }
}
