using EvrenDev.Application.Features.Donations.BrunnenDonations.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.BrunnenDonations.Queries.GetBrunnenDonationById;

public class GetBrunnenDonationByIdQuery : IRequest<Result<FullBrunnenDonationDto>>
{
    public Guid Id { get; set; }
}

public class GetBrunnenDonationByIdQueryValidator : AbstractValidator<GetBrunnenDonationByIdQuery>
{
    private readonly IStringLocalizer<GetBrunnenDonationByIdQueryValidator> _localizer;

    public GetBrunnenDonationByIdQueryValidator(IStringLocalizer<GetBrunnenDonationByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.get.id.required"]);
    }
}

public class GetBrunnenDonationByIdQueryHandler : IRequestHandler<GetBrunnenDonationByIdQuery, Result<FullBrunnenDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<GetBrunnenDonationByIdQueryHandler> _localizer;

    public GetBrunnenDonationByIdQueryHandler(
        IDonationDbContext context,
        IStringLocalizer<GetBrunnenDonationByIdQueryHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<FullBrunnenDonationDto>> Handle(GetBrunnenDonationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BrunnenDonations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var donation = new FullBrunnenDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            Phone = entity.Phone,
            ProjectCode = entity.ProjectCode,
            Banner = entity.Banner,
            ProjectNumber = entity.ProjectNumber,
            Project = entity.Project,
            TransactionId = entity.TransactionId,
            Source = entity.Source,
            CreationDate = DateTimeDto.Create.FromUtc(entity.Date),
            Info = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}"
        };

        return Result<FullBrunnenDonationDto>.Success(donation);
    }
}
