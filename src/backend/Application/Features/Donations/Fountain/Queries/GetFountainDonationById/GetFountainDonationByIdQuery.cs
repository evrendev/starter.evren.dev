using Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Queries.GetFountainDonationById;

public class GetFountainDonationByIdQuery : IRequest<Result<FullFountainDonationDto>>
{
    public Guid Id { get; set; }
}

public class GetFountainDonationByIdQueryValidator : AbstractValidator<GetFountainDonationByIdQuery>
{
    private readonly IStringLocalizer<GetFountainDonationByIdQueryValidator> _localizer;

    public GetFountainDonationByIdQueryValidator(IStringLocalizer<GetFountainDonationByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.get.id.required"]);
    }
}

public class GetFountainDonationByIdQueryHandler : IRequestHandler<GetFountainDonationByIdQuery, Result<FullFountainDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<GetFountainDonationByIdQueryHandler> _localizer;

    public GetFountainDonationByIdQueryHandler(
        IDonationDbContext context,
        IStringLocalizer<GetFountainDonationByIdQueryHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<FullFountainDonationDto>> Handle(GetFountainDonationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var donation = new FullFountainDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            Phone = !string.IsNullOrEmpty(entity.Phone) ? Tools.FormatPhoneNumber(entity.Phone) : null,
            ProjectCode = entity.ProjectCode,
            Banner = entity.Banner,
            ProjectNumber = entity.ProjectNumber,
            Project = entity.Project,
            TransactionId = entity.TransactionId,
            Source = entity.Source,
            CreationDate = DateTimeDto.Create.FromUtc(entity.CreationDate),
            Info = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}",
            Team = entity.Team,
        };

        return Result<FullFountainDonationDto>.Success(donation);
    }
}


