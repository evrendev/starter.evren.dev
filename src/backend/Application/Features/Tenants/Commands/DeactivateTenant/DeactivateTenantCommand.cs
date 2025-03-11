using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Commands.DeactivateTenant;

public class DeactivateTenantCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class DeactivateTenantCommandValidator : AbstractValidator<DeactivateTenantCommand>
{
    private readonly IStringLocalizer<DeactivateTenantCommandValidator> _localizer;

    public DeactivateTenantCommandValidator(IStringLocalizer<DeactivateTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.deactivate.id.required"]);
    }
}

public class DeactivateTenantCommandHandler : IRequestHandler<DeactivateTenantCommand, Result<bool>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<DeactivateTenantCommandHandler> _localizer;

    public DeactivateTenantCommandHandler(
        ITenantDbContext context,
        IStringLocalizer<DeactivateTenantCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return Result<bool>.Failure(_localizer["api.tenants.not-found"]);

        entity.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
