using Application.Common.Functions;
using EvrenDev.Application.Features.Donations.BrunnenDonations.Models;
using EvrenDev.Domain.Entities.Donation;

namespace EvrenDev.Application.Features.Donations.BrunnenDonations.Queries.GetBrunnenDonations;

public class GetBrunnenDonationsQuery : IRequest<Result<PaginatedList<BasicBrunnenDonationDto>>>
{
    public string? Search { get; set; }
    public string? ProjectCode { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 25;
    public string? SortBy { get; init; }
    public string? SortDesc { get; init; }
}

public class GetBrunnenDonationsQueryValidator : AbstractValidator<GetBrunnenDonationsQuery>
{
    private readonly IStringLocalizer<GetBrunnenDonationsQueryValidator> _localizer;

    public GetBrunnenDonationsQueryValidator(IStringLocalizer<GetBrunnenDonationsQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.StartDate)
            .LessThan(v => v.EndDate)
            .When(v => v.StartDate.HasValue && v.EndDate.HasValue)
            .WithMessage(_localizer["api.donations.startdate.less.than.enddate"]);

        RuleFor(v => v.EndDate)
            .GreaterThan(v => v.StartDate)
            .When(v => v.StartDate.HasValue && v.EndDate.HasValue)
            .WithMessage(_localizer["api.donations.enddate.greater.than.startdate"]);

        RuleFor(v => v.Page)
            .GreaterThan(0)
            .WithMessage(_localizer["api.donations.page.greater.than.zero"]);

        RuleFor(v => v.ItemsPerPage)
            .GreaterThan(0)
            .WithMessage(_localizer["api.donations.itemsperpage.greater.than.zero"]);
    }
}

public class GetBrunnenDonationsQueryHandler : IRequestHandler<GetBrunnenDonationsQuery, Result<PaginatedList<BasicBrunnenDonationDto>>>
{
    private readonly IDonationDbContext _context;

    public GetBrunnenDonationsQueryHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<BasicBrunnenDonationDto>>> Handle(GetBrunnenDonationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BrunnenDonations.AsQueryable();

        if (request.StartDate != null)
            query = query.Where(entity => entity.Date >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.Date <= request.EndDate);

        if (!string.IsNullOrEmpty(request.ProjectCode))
            query = query.Where(entity => entity.ProjectCode == request.ProjectCode);

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(entity =>
                entity.Contact != null && entity.Contact.Contains(request.Search)
                ||
                entity.Phone != null && entity.Phone.Contains(request.Search)
                ||
                entity.Banner != null && entity.Banner.Contains(request.Search)
                ||
                entity.Project != null && entity.Project.Contains(request.Search)
                ||
                entity.ProjectCode != null && entity.ProjectCode.Contains(request.Search)
                ||
                entity.TransactionId != null && entity.TransactionId.Contains(request.Search)
            );

        // Apply sorting
        query = !string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDesc)
            ? ApplySorting(query, request.SortBy, request.SortDesc == "desc")
            : query.OrderByDescending(x => x.Date);

        var dtoQuery = query.Select(entity => new BasicBrunnenDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            Phone = !string.IsNullOrEmpty(entity.Phone) ? Tools.FormatPhoneNumber(entity.Phone) : null,
            CreationDate = DateTimeDto.Create.FromUtc(entity.Date),
            HtmlBanner = $"<strong>{entity.ProjectCode}{entity.ProjectNumber}:</strong> {entity.Banner}",
            PlainBanner = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}",
            Team = entity.Team,
        });

        var paginatedList = await PaginatedList<BasicBrunnenDonationDto>.CreateAsync(
            source: dtoQuery,
            page: request.Page,
            itemsPerPage: request.ItemsPerPage);

        return Result<PaginatedList<BasicBrunnenDonationDto>>.Success(paginatedList);
    }

    private static IQueryable<BrunnenDonation> ApplySorting(IQueryable<BrunnenDonation> query, string sortBy, bool sortDesc)
    {
        return sortBy.ToLower() switch
        {
            "id" => sortDesc
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id),
            "contact" => sortDesc
                ? query.OrderByDescending(x => x.Contact)
                : query.OrderBy(x => x.Contact),
            "projectcode" => sortDesc
                ? query.OrderByDescending(x => x.ProjectCode)
                : query.OrderBy(x => x.ProjectCode),
            "info" => sortDesc
                ? query.OrderByDescending(x => x.ProjectNumber)
                : query.OrderBy(x => x.ProjectNumber),
            _ => query.OrderByDescending(x => x.Date) // Default sorting
        };
    }
}
