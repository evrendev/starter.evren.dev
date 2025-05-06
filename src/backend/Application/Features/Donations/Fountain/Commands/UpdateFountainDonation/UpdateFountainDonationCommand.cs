namespace EvrenDev.Application.Features.Donations.Fountain.Commands.UpdateFountainDonation;

public class UpdateFountainDonationCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string? Banner { get; set; }
    public string? Contact { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Phone { get; set; }
    public string? Project { get; set; }
}

public class UpdateFountainDonationCommandValidator : AbstractValidator<UpdateFountainDonationCommand>
{
    private readonly IStringLocalizer<UpdateFountainDonationCommandValidator> _localizer;

    public UpdateFountainDonationCommandValidator(IStringLocalizer<UpdateFountainDonationCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.donations.fountain.update.id.required"]);

        RuleFor(x => x.Contact)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.contact.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fountain.update.contact.maxlength"]);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.phone.required"])
            .MaximumLength(100)
            .WithMessage(_localizer["api.donations.fountain.update.phone.maxlength"]);

        RuleFor(x => x.Banner)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.banner.required"])
            .MaximumLength(1000)
            .WithMessage(_localizer["api.donations.fountain.update.banner.maxlength"]);

        RuleFor(x => x.Project)
            .NotEmpty()
            .WithMessage(_localizer["api.donations.fountain.update.project.required"])
            .Must(code => FountainDonationProject.ToList.Select(pc => pc.Name).Contains(code))
            .WithMessage(_localizer["api.donations.fountain.update.project.invalid"]);

        RuleFor(v => v.CreationDate)
            .NotNull()
            .WithMessage(_localizer["api.donations.fountain.update.creation-date.required"]);
    }
}

public class UpdateFountainDonationCommandHandler : IRequestHandler<UpdateFountainDonationCommand, Result<Guid>>
{
    private readonly IDonationDbContext _context;

    public UpdateFountainDonationCommandHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdateFountainDonationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations.FindAsync([request.Id], cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        entity.Banner = request.Banner!.Trim();
        entity.Contact = request.Contact!.Trim();
        entity.CreationDate = request.CreationDate ?? DateTime.UtcNow;
        entity.Phone = request.Phone;
        entity.Project = request.Project;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
