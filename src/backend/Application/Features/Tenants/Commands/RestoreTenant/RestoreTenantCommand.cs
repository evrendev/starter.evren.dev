using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Commands.RestoreTenant;

public class RestoreTenantCommand : IRequest<Result<bool>>
{
    public string? Id { get; set; }
}

public class RestoreTenantCommandValidator : AbstractValidator<RestoreTenantCommand>
{
    private readonly IStringLocalizer<RestoreTenantCommandValidator> _localizer;

    public RestoreTenantCommandValidator(IStringLocalizer<RestoreTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.restore.id.required"]);
    }
}

public class RestoreTenantCommandHandler : IRequestHandler<RestoreTenantCommand, Result<bool>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<RestoreTenantCommandHandler> _localizer;

    public RestoreTenantCommandHandler(
        ITenantDbContext context,
        IStringLocalizer<RestoreTenantCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(RestoreTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return Result<bool>.Failure(_localizer["api.tenants.not-found"]);

        if (!entity.Deleted)
            return Result<bool>.Failure(_localizer["api.tenants.not-deleted"]);

        entity.Restore();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
