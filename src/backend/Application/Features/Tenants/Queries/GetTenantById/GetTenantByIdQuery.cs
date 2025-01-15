using EvrenDev.Application.Features.Tenants.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdQuery : IRequest<Result<TenantDto>>
{
    public Guid Id { get; set; }
}

public class GetTenantByIdQueryValidator : AbstractValidator<GetTenantByIdQuery>
{
    private readonly IStringLocalizer<GetTenantByIdQueryValidator> _localizer;

    public GetTenantByIdQueryValidator(IStringLocalizer<GetTenantByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.get.id.required"]);
    }
}

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, Result<TenantDto>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<GetTenantByIdQueryHandler> _localizer;

    public GetTenantByIdQueryHandler(
        ITenantDbContext context,
        IStringLocalizer<GetTenantByIdQueryHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<TenantDto>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var tenant = new TenantDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ConnectionString = entity.ConnectionString,
            Host = entity.Host,
            IsActive = entity.IsActive,
            AdminEmail = entity.AdminEmail,
            ValidUntil = entity.ValidUntil,
            Description = entity.Description,
            Deleted = entity.Deleted
        };

        return Result<TenantDto>.Success(tenant);
    }
}
