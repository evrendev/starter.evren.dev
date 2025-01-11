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
        var tenant = await _context.Tenants
            .Select(x => new TenantDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (tenant == null)
            return Result<TenantDto>.Failure(_localizer["api.tenants.not-found"]);

        return Result<TenantDto>.Success(tenant);
    }
}
