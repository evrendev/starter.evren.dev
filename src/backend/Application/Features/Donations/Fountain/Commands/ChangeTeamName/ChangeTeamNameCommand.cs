using EvrenDev.Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeTeamName;

public class ChangeTeamNameCommand : IRequest<Result<BasicFountainDonationDto>>
{
    public Guid Id { get; set; }

    public string? TeamName { get; set; }
}

public class ChangeTeamNameCommandValidator : AbstractValidator<ChangeTeamNameCommand>
{
    private readonly IStringLocalizer<ChangeTeamNameCommandValidator> _localizer;

    public ChangeTeamNameCommandValidator(IStringLocalizer<ChangeTeamNameCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);

        RuleFor(x => x.TeamName)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.team.required"]);
    }
}

public class ChangeTeamNameCommandHandler : IRequestHandler<ChangeTeamNameCommand, Result<BasicFountainDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<ChangeTeamNameCommandHandler> _localizer;

    public ChangeTeamNameCommandHandler(
        IDonationDbContext context,
        IStringLocalizer<ChangeTeamNameCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<BasicFountainDonationDto>> Handle(ChangeTeamNameCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(FountainDonation), request.Id.ToString());

        entity.Team = request.TeamName;
        await _context.SaveChangesAsync(cancellationToken);

        var response = new BasicFountainDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            Phone = Tools.CreatePhone(entity.Phone, $"{entity.ProjectCode}{entity.ProjectNumber}", entity.Banner),
            CreationDate = DateTimeDto.Create.FromUtc(entity.CreationDate),
            HtmlBanner = $"<strong>{entity.ProjectCode}{entity.ProjectNumber}:</strong> {entity.Banner}",
            PlainBanner = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}",
            Team = FountaionTeam.From(entity.Team),
            MediaStatus = MediaStatus.From(entity.MediaStatus),
        };

        return Result<BasicFountainDonationDto>.Success(response);
    }
}
