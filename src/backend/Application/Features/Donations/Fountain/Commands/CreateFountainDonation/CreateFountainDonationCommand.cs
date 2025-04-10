using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.CreateFountainDonation;

public class CreateFountainDonationCommand : IRequest<Result<Guid>>
{
    public string? Banner { get; set; }
    public string? Contact { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Phone { get; set; }
    public string? ProjectCode { get; set; }
}

public class CreateFountainDonationCommandValidator : AbstractValidator<CreateFountainDonationCommand>
{
    private readonly IStringLocalizer<CreateFountainDonationCommandValidator> _localizer;

    public CreateFountainDonationCommandValidator(IStringLocalizer<CreateFountainDonationCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(x => x.Contact)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.create.contact.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fountain.create.contact.maxlength"]);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.create.phone.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fountain.create.phone.maxlength"]);

        RuleFor(x => x.Banner)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.create.banner.required"])
            .MaximumLength(1000)
            .WithMessage(_localizer["api.donations.fountain.create.banner.maxlength"]);

        RuleFor(x => x.ProjectCode)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.create.project-code.required"])
            .Must(code => FountainDonationProject.ToList.Select(pc => pc.Name).Contains(code))
            .WithMessage(_localizer["api.donations.fountain.create.project-code.invalid"]);

        RuleFor(v => v.CreationDate)
            .NotNull()
            .WithMessage(_localizer["api.donations.fountain.create.creation-date.required"]);
    }
}

public class CreateFountainDonationCommandHandler : IRequestHandler<CreateFountainDonationCommand, Result<Guid>>
{
    private readonly IDonationDbContext _context;

    public CreateFountainDonationCommandHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateFountainDonationCommand request, CancellationToken cancellationToken)
    {
        var lastDonation = await _context.FountainDonations
            .Where(x => x.ProjectCode == request.ProjectCode)
            .OrderByDescending(x => x.ProjectNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var projectNumber = lastDonation?.ProjectNumber + 1 ?? 1;

        var entity = new FountainDonation
        {
            Banner = request.Banner,
            Contact = request.Contact,
            CreationDate = request.CreationDate ?? DateTime.UtcNow,
            Phone = request.Phone,
            ProjectCode = request.ProjectCode,
            Project = FountainDonationProject.From(request.ProjectCode).Alias,
            ProjectNumber = projectNumber,
            Source = "MANUEL",
        };

        _context.FountainDonations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
