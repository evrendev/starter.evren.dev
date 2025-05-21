using EvrenDev.Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Queries.GetDonationsOverview;

public class GetDonationsOverviewQuery : IRequest<Result<DonationOverview?>>
{
    public string? Project { get; init; }
    public DateTime? StartDate { get; init; } = new DateTime(DateTime.Now.Year, 1, 1);
    public DateTime? EndDate { get; init; } = new DateTime(DateTime.Now.Year, 12, 31);
}

public class GetDonationsOverviewQueryValidator : AbstractValidator<GetDonationsOverviewQuery>
{
    private readonly IStringLocalizer<GetDonationsOverviewQueryValidator> _localizer;

    public GetDonationsOverviewQueryValidator(IStringLocalizer<GetDonationsOverviewQueryValidator> localizer)
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

public class GetDonationsOverviewQueryHandler : IRequestHandler<GetDonationsOverviewQuery, Result<DonationOverview?>>
{
    private readonly IDonationDbContext _context;

    public GetDonationsOverviewQueryHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DonationOverview?>> Handle(GetDonationsOverviewQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FountainDonations.AsQueryable().Where(donation => donation.Source != "EMPTY");

        if (request.StartDate != null)
            query = query.Where(entity => entity.CreationDate >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.CreationDate <= request.EndDate);

        if (!string.IsNullOrEmpty(request.Project))
            query = query.Where(entity => entity.Project == request.Project);

        var stats = await query
            .GroupBy(x => x.Project)
            .Select(g => new ProjectsCountDto
            {
                Project = FountainDonationProject.FromName(g.Key),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var groupedDonations = await query
            .GroupBy(d => d.Project)
            .ToListAsync(cancellationToken);

        var donations = groupedDonations
            .SelectMany(g => g
                .OrderByDescending(d => d.CreationDate)
                .Take(3))
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

        var response = new DonationOverview()
        {
            Stats = stats,
            Donations = donations,
        };

        return Result<DonationOverview?>.Success(response);
    }
}
