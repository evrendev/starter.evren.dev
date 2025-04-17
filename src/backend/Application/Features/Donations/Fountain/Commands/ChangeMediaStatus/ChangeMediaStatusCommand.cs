using EvrenDev.Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.ChangeMediaStatus;

public class ChangeMediaStatusCommand : IRequest<Result<BasicFountainDonationDto>>
{
    public Guid Id { get; set; }

    public string? MediaStatus { get; set; }
}

public class ChangeMediaStatusCommandValidator : AbstractValidator<ChangeMediaStatusCommand>
{
    private readonly IStringLocalizer<ChangeMediaStatusCommandValidator> _localizer;

    public ChangeMediaStatusCommandValidator(IStringLocalizer<ChangeMediaStatusCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);

        RuleFor(x => x.MediaStatus)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.media-status.required"])
            .Must(code => MediaStatus.ToList.Select(status => status.Name).Contains(code))
            .WithMessage(_localizer["api.donations.fountain.update.media-status.invalid"]);
    }
}

public class ChangeMediaStatusCommandHandler : IRequestHandler<ChangeMediaStatusCommand, Result<BasicFountainDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<ChangeMediaStatusCommandHandler> _localizer;

    public ChangeMediaStatusCommandHandler(
        IDonationDbContext context,
        IStringLocalizer<ChangeMediaStatusCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<BasicFountainDonationDto>> Handle(ChangeMediaStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(FountainDonation), request.Id.ToString());

        entity.MediaStatus = request.MediaStatus;
        await _context.SaveChangesAsync(cancellationToken);

        var response = new BasicFountainDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            Phone = Tools.CreatePhone(number: entity.Phone, message: $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}"),
            CreationDate = DateTimeDto.Create.FromUtc(entity.CreationDate),
            HtmlBanner = $"<strong>{entity.ProjectCode}{entity.ProjectNumber}:</strong> {entity.Banner}",
            PlainBanner = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}",
            Team = entity.Team,
            MediaStatus = MediaStatus.From(entity.MediaStatus),
        };

        return Result<BasicFountainDonationDto>.Success(response);
    }
}
