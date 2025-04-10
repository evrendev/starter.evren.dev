using EvrenDev.Domain.Entities.Donation;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.DeleteDonation;

public class DeleteDonationCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteDonationCommandValidator : AbstractValidator<DeleteDonationCommand>
{
    private readonly IStringLocalizer<DeleteDonationCommandValidator> _localizer;

    public DeleteDonationCommandValidator(IStringLocalizer<DeleteDonationCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.donations.fountain.delete.id.required"]);
    }
}

public class DeleteDonationCommandHandler : IRequestHandler<DeleteDonationCommand, Result<bool>>
{
    private readonly IDonationDbContext _context;
    private readonly IStringLocalizer<DeleteDonationCommandHandler> _localizer;

    public DeleteDonationCommandHandler(
        IDonationDbContext context,
        IStringLocalizer<DeleteDonationCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeleteDonationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.FountainDonations.FindAsync([request.Id], cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(FountainDonation), request.Id.ToString());

        _context.FountainDonations.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
