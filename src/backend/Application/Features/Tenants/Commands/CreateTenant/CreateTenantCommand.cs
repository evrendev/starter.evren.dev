using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommand : IRequest<Result<Guid>>
{
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
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
    }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<Guid>>
{
    private readonly ITenantDbContext _context;

    public CreateTenantCommandHandler(ITenantDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = new TenantEntity
        {
            Name = request.Name,
            ConnectionString = request.ConnectionString,
            Host = request.Host,
            IsActive = request.IsActive
        };

        _context.Tenants.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
