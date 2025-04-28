using EvrenDev.Application.Features.Donations.Fountain.Models;
using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.CreateEmptyDonation;

public class CreateEmptyDonationCommand : IRequest<Result<BasicFountainDonationDto>>
{
    public string? ProjectCode { get; set; }
    public string? Team { get; set; }
}

public class CreateEmptyDonationCommandValidator : AbstractValidator<CreateEmptyDonationCommand>
{
    private readonly IStringLocalizer<CreateEmptyDonationCommandValidator> _localizer;

    public CreateEmptyDonationCommandValidator(IStringLocalizer<CreateEmptyDonationCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(x => x.Team)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.create.team.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fountain.create.team.maxlength"]);

        RuleFor(x => x.ProjectCode)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.create.project-code.required"])
            .Must(code => FountainDonationProject.ToList.Select(pc => pc.Name).Contains(code))
            .WithMessage(_localizer["api.donations.fountain.create.project-code.invalid"]);
    }
}

public class CreateEmptyDonationCommandHandler : IRequestHandler<CreateEmptyDonationCommand, Result<BasicFountainDonationDto>>
{
    private readonly IDonationDbContext _context;

    public CreateEmptyDonationCommandHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BasicFountainDonationDto>> Handle(CreateEmptyDonationCommand request, CancellationToken cancellationToken)
    {
        var lastDonation = await _context.FountainDonations
            .Where(x => x.ProjectCode == request.ProjectCode)
            .OrderByDescending(x => x.ProjectNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var projectNumber = lastDonation?.ProjectNumber + 1 ?? 1;

        var entity = new FountainDonation
        {
            Banner = string.Empty,
            Contact = string.Empty,
            CreationDate = DateTime.Now,
            Phone = string.Empty,
            ProjectCode = request.ProjectCode,
            Project = FountainDonationProject.FromName(request.ProjectCode).Alias,
            ProjectNumber = projectNumber,
            Team = request.Team,
            MediaStatus = MediaStatus.None.Name,
            TransactionId = string.Empty,
            Source = "EMPTY",
        };

        _context.FountainDonations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var donation = new BasicFountainDonationDto
        {
            Id = entity.Id,
            Contact = entity.Contact,
            CreationDate = DateTimeDto.Create.FromUtc(entity.CreationDate),
            HtmlBanner = $"<strong>{entity.ProjectCode}{entity.ProjectNumber}:</strong> {entity.Banner}",
            PlainBanner = $"{entity.ProjectCode}{entity.ProjectNumber}: {entity.Banner}",
            Team = FountaionTeam.From(entity.Team),
            MediaStatus = MediaStatus.From(entity.MediaStatus),
            IsDonorNotified = entity.IsDonorNotified,
            IsConstructionTeamNotified = entity.IsConstructionTeamNotified
        };


        return Result<BasicFountainDonationDto>.Success(donation);
    }
}
