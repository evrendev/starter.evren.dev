using Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Domain.Entities.Donation;

namespace EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonations;

public class GetFountainDonationsQuery : IRequest<Result<PaginatedList<BasicFountainDonationDto>>>
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

public class GetFountainDonationsQueryValidator : AbstractValidator<GetFountainDonationsQuery>
{
    private readonly IStringLocalizer<GetFountainDonationsQueryValidator> _localizer;

    public GetFountainDonationsQueryValidator(IStringLocalizer<GetFountainDonationsQueryValidator> localizer)
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

        RuleFor(v => v.Page)
            .GreaterThan(0)
            .WithMessage(_localizer["api.donations.fountains.page.greater.than.zero"]);

        RuleFor(v => v.ItemsPerPage)
            .GreaterThan(0)
            .WithMessage(_localizer["api.donations.fountains.itemsperpage.greater.than.zero"]);
    }
}

public class GetFountainDonationsQueryHandler : IRequestHandler<GetFountainDonationsQuery, Result<PaginatedList<BasicFountainDonationDto>>>
{
    private readonly IDonationDbContext _context;

    public GetFountainDonationsQueryHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<BasicFountainDonationDto>>> Handle(GetFountainDonationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.FountainDonations.AsQueryable();

        if (request.StartDate != null)
            query = query.Where(entity => entity.CreationDate >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.CreationDate <= request.EndDate);

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
            : query.OrderByDescending(x => x.CreationDate);

        var dtoQuery = query.Select(entity => new BasicFountainDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            Phone = !string.IsNullOrEmpty(entity.Phone) ? Tools.FormatPhoneNumber(entity.Phone) : null,
            CreationDate = DateTimeDto.Create.FromUtc(entity.CreationDate),
            HtmlBanner = $"<strong>{entity.ProjectCode}{entity.ProjectNumber}:</strong> {entity.Banner}",
            PlainBanner = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}",
            Team = entity.Team,
            MediaStatus = MediaStatus.From(entity.MediaStatus),
        });

        var paginatedList = await PaginatedList<BasicFountainDonationDto>.CreateAsync(
            source: dtoQuery,
            page: request.Page,
            itemsPerPage: request.ItemsPerPage);

        return Result<PaginatedList<BasicFountainDonationDto>>.Success(paginatedList);
    }

    private static IQueryable<FountainDonation> ApplySorting(IQueryable<FountainDonation> query, string sortBy, bool sortDesc)
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
            _ => query.OrderByDescending(x => x.CreationDate) // Default sorting
        };
    }
}
