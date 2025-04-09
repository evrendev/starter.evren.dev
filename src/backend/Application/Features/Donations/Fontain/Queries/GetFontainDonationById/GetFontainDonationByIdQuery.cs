using Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fontain.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fontain.Queries.GetFontainDonationById;

public class GetFontainDonationByIdQuery : IRequest<Result<FullFontainDonationDto>>
{
    public Guid Id { get; set; }
}

public class GetFontainDonationByIdQueryValidator : AbstractValidator<GetFontainDonationByIdQuery>
{
    private readonly IStringLocalizer<GetFontainDonationByIdQueryValidator> _localizer;

    public GetFontainDonationByIdQueryValidator(IStringLocalizer<GetFontainDonationByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.get.id.required"]);
    }
}

public class GetFontainDonationByIdQueryHandler : IRequestHandler<GetFontainDonationByIdQuery, Result<FullFontainDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<GetFontainDonationByIdQueryHandler> _localizer;

    public GetFontainDonationByIdQueryHandler(
        IDonationDbContext context,
        IStringLocalizer<GetFontainDonationByIdQueryHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<FullFontainDonationDto>> Handle(GetFontainDonationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.FontainDonations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var donation = new FullFontainDonationDto
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

        return Result<FullFontainDonationDto>.Success(donation);
    }
}


