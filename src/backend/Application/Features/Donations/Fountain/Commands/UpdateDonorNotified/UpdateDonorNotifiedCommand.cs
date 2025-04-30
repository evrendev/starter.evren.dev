using EvrenDev.Application.Common.Functions;
using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.UpdateDonorNotified;

public class UpdateDonorNotifiedCommand : IRequest<Result<BasicFountainDonationDto>>
{
    public Guid Id { get; set; }
}

public class UpdateDonorNotifiedCommandValidator : AbstractValidator<UpdateDonorNotifiedCommand>
{
    private readonly IStringLocalizer<UpdateDonorNotifiedCommandValidator> _localizer;

    public UpdateDonorNotifiedCommandValidator(IStringLocalizer<UpdateDonorNotifiedCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);
    }
}

public class UpdateDonorNotifiedCommandHandler : IRequestHandler<UpdateDonorNotifiedCommand, Result<BasicFountainDonationDto>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<UpdateDonorNotifiedCommandHandler> _localizer;

    public UpdateDonorNotifiedCommandHandler(
        IDonationDbContext context,
        IStringLocalizer<UpdateDonorNotifiedCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<BasicFountainDonationDto>> Handle(UpdateDonorNotifiedCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(FountainDonation), request.Id.ToString());

        entity.IsDonorNotified = true;

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
            IsDonorNotified = entity.IsDonorNotified,
            IsConstructionTeamNotified = entity.IsConstructionTeamNotified
        };

        return Result<BasicFountainDonationDto>.Success(response);
    }
}
