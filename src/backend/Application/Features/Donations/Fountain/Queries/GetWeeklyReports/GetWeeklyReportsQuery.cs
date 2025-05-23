using EvrenDev.Application.Features.Donations.Fountain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Application.Features.Donations.Fountain.Queries.GetWeeklyReports;

public class GetWeeklyReportsQuery : IRequest<Result<WeeklyReportDto?>>
{
}

public class GetWeeklyReportsQueryHandler : IRequestHandler<GetWeeklyReportsQuery, Result<WeeklyReportDto?>>
{
    private readonly IDonationDbContext _context;
    private readonly ILogger<GetWeeklyReportsQueryHandler> _logger;

    public GetWeeklyReportsQueryHandler(IDonationDbContext context,
        ILogger<GetWeeklyReportsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<WeeklyReportDto?>> Handle(GetWeeklyReportsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.FountainDonations.AsQueryable().Where(donation => donation.Source != "EMPTY");
            var projects = new List<ProjectReportDto>();

            foreach (var project in FountainDonationProject.ToList)
            {
                var lastOnlineFountain = await query
                    .Where(d => d.Project == project.Name && d.MediaStatus == MediaStatus.Online.Name)
                    .OrderByDescending(d => d.CreationDate)
                    .FirstOrDefaultAsync(cancellationToken);

                var pendingMediaFountains = await query
                    .Where(d =>
                        d.Project == project.Name
                        &&
                        (d.MediaStatus == MediaStatus.None.Name || d.MediaStatus == MediaStatus.Missing.Name)
                    )
                    .ToListAsync(cancellationToken);

                pendingMediaFountains = pendingMediaFountains
                    .Where(d => GetWeekNumber(d.CreationDate) == 7)
                    .OrderBy(d => d.CreationDate)
                    .ToList();

                var lastAssignedFountain = await query
                    .Where(d => d.Project == project.Name)
                    .OrderByDescending(d => d.CreationDate)
                    .FirstOrDefaultAsync(cancellationToken);

                var missingSince6Weeks = await query
                    .Where(d =>
                        d.Project == project.Name
                        &&
                        (
                            d.MediaStatus == MediaStatus.None.Name
                            ||
                            d.MediaStatus == MediaStatus.Missing.Name
                        )
                    )
                    .ToListAsync(cancellationToken);

                missingSince6Weeks = missingSince6Weeks
                    .Where(d => GetWeekNumber(d.CreationDate) == 6)
                    .OrderBy(d => d.CreationDate)
                    .ToList();

                var missingSince8Weeks = await query
                    .Where(d =>
                        d.Project == project.Name
                        &&
                        (
                            d.MediaStatus == MediaStatus.None.Name
                            ||
                            d.MediaStatus == MediaStatus.Missing.Name
                        )
                    )
                    .ToListAsync(cancellationToken);

                missingSince8Weeks = missingSince8Weeks
                    .Where(d => GetWeekNumber(d.CreationDate) == 8)
                    .OrderBy(d => d.CreationDate)
                    .ToList();

                var missingSince13Weeks = await query
                    .Where(d =>
                        d.Project == project.Name
                        &&
                        (
                            d.MediaStatus == MediaStatus.None.Name
                            ||
                            d.MediaStatus == MediaStatus.Missing.Name
                        )
                    )
                    .ToListAsync(cancellationToken);

                missingSince13Weeks = missingSince13Weeks
                    .Where(d => GetWeekNumber(d.CreationDate) == 13)
                    .OrderBy(d => d.CreationDate)
                    .ToList();

                projects.Add(new ProjectReportDto
                {
                    Project = project,
                    LastOnlineFountain = new FountainItemDto
                    {
                        Code = $"{lastOnlineFountain?.Project}-{lastOnlineFountain?.ProjectNumber}",
                        CreationDate = lastOnlineFountain?.CreationDate,
                    },
                    PendingMediaFountains = pendingMediaFountains.Select(d => new FountainItemDto
                    {
                        Code = $"{d.Project}-{d.ProjectNumber}",
                        CreationDate = d.CreationDate,
                    }).ToList(),
                    LastAssignedFountain = new FountainItemDto
                    {
                        Code = $"{lastAssignedFountain?.Project}-{lastAssignedFountain?.ProjectNumber}",
                        CreationDate = lastAssignedFountain?.CreationDate,
                    },
                    MissingSince6Weeks = missingSince6Weeks.Select(d => new FountainItemDto
                    {
                        Code = $"{d.Project}-{d.ProjectNumber}",
                        CreationDate = d.CreationDate,
                    }).ToList(),
                    MissingSince8Weeks = missingSince8Weeks.Select(d => new FountainItemDto
                    {
                        Code = $"{d.Project}-{d.ProjectNumber}",
                        CreationDate = d.CreationDate,
                    }).ToList(),
                    MissingSince13Weeks = missingSince13Weeks.Select(d => new FountainItemDto
                    {
                        Code = $"{d.Project}-{d.ProjectNumber}",
                        CreationDate = d.CreationDate,
                    }).ToList(),
                });
            }

            var report = new WeeklyReportDto
            {
                Projects = projects,
            };

            return Result<WeeklyReportDto?>.Success(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting donations overview");
            return Result<WeeklyReportDto?>.Failure("api.donations.fountains.overview.error");
            throw;
        }
    }

    private int GetWeekNumber(DateTime? creationDate)
    {
        return creationDate is null ? 0 : (int)Math.Round((DateTime.Today - creationDate.Value).TotalDays / 7.0);
    }
}
