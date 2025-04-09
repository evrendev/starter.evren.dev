using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fontain.Commands.CreateFontainDonation;

public class CreateFontainDonationCommand : IRequest<Result<Guid>>
{
    public string? Banner { get; set; }
    public string? Contact { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Phone { get; set; }
    public string? ProjectCode { get; set; }
}

public class CreateFontainDonationCommandValidator : AbstractValidator<CreateFontainDonationCommand>
{
    private readonly IStringLocalizer<CreateFontainDonationCommandValidator> _localizer;

    public CreateFontainDonationCommandValidator(IStringLocalizer<CreateFontainDonationCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(x => x.Contact)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fontain.create.contact.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fontain.create.contact.maxlength"]);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fontain.create.phone.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fontain.create.phone.maxlength"]);

        RuleFor(x => x.Banner)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fontain.create.banner.required"])
            .MaximumLength(1000)
            .WithMessage(_localizer["api.donations.fontain.create.banner.maxlength"]);

        RuleFor(x => x.ProjectCode)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fontain.create.projectCode.required"])
            .Must(code => new[] { "BKS", "BGS", "AKI", "AGI" }.Contains(code))
            .WithMessage(_localizer["api.donations.fontain.create.projectCode.invalid"]);

        RuleFor(v => v.CreationDate)
            .NotNull()
            .WithMessage(_localizer["api.donations.fontain.create.creationdate.required"])
            .When(v => v.CreationDate.HasValue);
    }
}

public class CreateFontainDonationCommandHandler : IRequestHandler<CreateFontainDonationCommand, Result<Guid>>
{
    private readonly IDonationDbContext _context;

    public CreateFontainDonationCommandHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateFontainDonationCommand request, CancellationToken cancellationToken)
    {
        var lastDonation = await _context.FontainDonations
            .Where(x => x.ProjectCode == request.ProjectCode)
            .OrderByDescending(x => x.ProjectNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var projectNumber = lastDonation?.ProjectNumber + 1 ?? 1;

        var entity = new FontainDonation
        {
            Banner = request.Banner,
            Contact = request.Contact,
            CreationDate = request.CreationDate ?? DateTime.UtcNow,
            Phone = request.Phone,
            ProjectCode = request.ProjectCode,
            Project = FontainDonationProject.From(request.ProjectCode).Alias,
            ProjectNumber = projectNumber,
            Source = "MANUEL",
        };

        _context.FontainDonations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
