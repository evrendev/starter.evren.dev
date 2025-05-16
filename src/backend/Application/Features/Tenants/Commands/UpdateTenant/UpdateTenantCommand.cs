using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? AdminEmail { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Description { get; set; }
}

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    private readonly IStringLocalizer<UpdateTenantCommandValidator> _localizer;

    public UpdateTenantCommandValidator(IStringLocalizer<UpdateTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.update.id.required"]);

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_localizer["api.tenants.update.name.required"])
            .MaximumLength(200).WithMessage(_localizer["api.tenants.update.name.maxlength"]);

        RuleFor(v => v.AdminEmail)
            .EmailAddress().WithMessage(_localizer["api.tenants.update.admin-email.invalid"])
            .When(v => !string.IsNullOrWhiteSpace(v.AdminEmail));

        RuleFor(v => v.ValidUntil)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage(_localizer["api.tenants.update.valid-until.future"])
            .When(v => v.ValidUntil.HasValue);

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage(_localizer["api.tenants.update.description.maxlength"])
            .When(v => !string.IsNullOrWhiteSpace(v.Description));
    }
}

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result<bool>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<UpdateTenantCommandHandler> _localizer;

    public UpdateTenantCommandHandler(
        ITenantDbContext context,
        IStringLocalizer<UpdateTenantCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(AppTenantInfo), request.Id.ToString());

        entity.Name = request.Name;
        entity.IsActive = request.IsActive;
        entity.AdminEmail = request.AdminEmail;
        entity.ValidUntil = request.ValidUntil;
        entity.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
