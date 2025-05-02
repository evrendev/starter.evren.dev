using System.Globalization;
using System.Text.Json.Serialization;
using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Donations.Fountain.Commands.CreateAutomaticFountainDonation;

public class CreateAutomaticFountainDonationCommand : IRequest<Result<string>>
{
    [JsonPropertyName("Kontakt")]
    public string? Contact { get; set; }

    [JsonPropertyName("Projekt")]
    public string? ProjectCode { get; set; }
    public string? Project
    {
        get
        {
            return ProjectCode?[..4];
        }
    }

    [JsonPropertyName("Getätigt")]
    public string? DateOfDonation { get; set; }
    public DateTime CreationDate
    {
        get
        {
            if (!string.IsNullOrEmpty(DateOfDonation))
            {
                var formattedDateOfDonation = DateOfDonation.Replace("\t", "").Trim();
                if (DateTime.TryParseExact(formattedDateOfDonation, "dd.MM.yyyy - HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return parsedDate;
                }
            }
            return DateTime.Now;
        }
    }

    [JsonPropertyName("Transaktions-ID")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("Telefon")]
    public string? Phone { get; set; }

    [JsonPropertyName("Öffentlich Spender-Name (als Nachweis auf den Spendenbannern)")]
    public string? Banner { get; set; }
}

public class CreateFountainDonationCommandValidator : AbstractValidator<CreateAutomaticFountainDonationCommand>
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
            .WithMessage(_localizer["api.donations.fountain.create.project-code.required"]);

        RuleFor(v => v.DateOfDonation)
            .NotNull()
            .WithMessage(_localizer["api.donations.fountain.create.creation-date.required"]);
    }
}

public class CreateAutomaticFountainDonationCommandHandler : IRequestHandler<CreateAutomaticFountainDonationCommand, Result<string>>
{
    private readonly IDonationDbContext _context;

    public CreateAutomaticFountainDonationCommandHandler(IDonationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(CreateAutomaticFountainDonationCommand request, CancellationToken cancellationToken)
    {
        var isTransactionIdExists = await _context.FountainDonations
            .AnyAsync(x => x.TransactionId == request.TransactionId, cancellationToken);

        if (isTransactionIdExists)
            return Result<string>.Failure("Exists");

        var lastDonation = await _context.FountainDonations
            .Where(x => x.Project == request.Project)
            .OrderByDescending(x => x.ProjectNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var projectNumber = lastDonation?.ProjectNumber + 1 ?? 1;

        var entity = new FountainDonation
        {
            Banner = request.Banner,
            Contact = request.Contact,
            Phone = request.Phone,
            ProjectCode = request.ProjectCode,
            Project = request.Project,
            CreationDate = request.CreationDate,
            ProjectNumber = projectNumber,
            Source = "AUTOMATIC",
        };

        _context.FountainDonations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success($"{entity.ProjectCode}-{entity.ProjectNumber}");
    }
}
