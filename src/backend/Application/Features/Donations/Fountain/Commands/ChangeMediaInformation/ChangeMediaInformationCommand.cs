using EvrenDev.Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeMediaInformation;

public class ChangeMediaInformationCommand : IRequest<Result<BasicFountainDonationDto>>
{
    public Guid Id { get; set; }

    public string? Status { get; set; }
    public string? Information { get; set; }
}

public class ChangeMediaInformationCommandValidator : AbstractValidator<ChangeMediaInformationCommand>
{
    private readonly IStringLocalizer<ChangeMediaInformationCommandValidator> _localizer;

    public ChangeMediaInformationCommandValidator(IStringLocalizer<ChangeMediaInformationCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.media-status.required"])
            .Must(code => MediaStatus.ToList.Select(status => status.Name).Contains(code))
            .WithMessage(_localizer["api.donations.fountain.update.media-status.invalid"]);
    }
}

public class ChangeMediaInformationCommandHandler : IRequestHandler<ChangeMediaInformationCommand, Result<BasicFountainDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<ChangeMediaInformationCommandHandler> _localizer;

    public ChangeMediaInformationCommandHandler(
        IDonationDbContext context,
        IStringLocalizer<ChangeMediaInformationCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<BasicFountainDonationDto>> Handle(ChangeMediaInformationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(FountainDonation), request.Id.ToString());

        entity.MediaStatus = request.Status;
        entity.MediaInformation = request.Information;
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
            MediaInformation = entity.MediaInformation,
        };

        return Result<BasicFountainDonationDto>.Success(response);
    }
}
