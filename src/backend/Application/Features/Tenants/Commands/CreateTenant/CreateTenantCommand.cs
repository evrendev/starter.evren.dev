using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommand : IRequest<Result<string?>>
{
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? AdminEmail { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Description { get; set; }
}

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    private readonly IStringLocalizer<CreateTenantCommandValidator> _localizer;

    public CreateTenantCommandValidator(IStringLocalizer<CreateTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_localizer["api.tenants.create.name.required"])
            .MaximumLength(200).WithMessage(_localizer["api.tenants.create.name.maxlength"]);

        RuleFor(v => v.AdminEmail)
            .EmailAddress().WithMessage(_localizer["api.tenants.create.admin-email.invalid"])
            .When(v => !string.IsNullOrWhiteSpace(v.AdminEmail));

        RuleFor(v => v.ValidUntil)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage(_localizer["api.tenants.create.valid-until.future"])
            .When(v => v.ValidUntil.HasValue);

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage(_localizer["api.tenants.create.description.maxlength"])
            .When(v => !string.IsNullOrWhiteSpace(v.Description));
    }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<string?>>
{
    private readonly ITenantDbContext _context;

    public CreateTenantCommandHandler(ITenantDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string?>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = new TenantEntity
        {
            Name = request.Name,
            IsActive = request.IsActive,
            AdminEmail = request.AdminEmail,
            ValidUntil = request.ValidUntil,
            Description = request.Description,
            Deleted = false
        };

        _context.Tenants.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string?>.Success(entity.Id);
    }
}
