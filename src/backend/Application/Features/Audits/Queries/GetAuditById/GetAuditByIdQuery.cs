using EvrenDev.Application.Features.Audits.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Audits.Queries.GetAuditById;

public class GetAuditByIdQuery : IRequest<Result<FullAuditDto>>
{
    public Guid Id { get; set; }
}

public class GetAuditByIdQueryValidator : AbstractValidator<GetAuditByIdQuery>
{
    private readonly IStringLocalizer<GetAuditByIdQueryValidator> _localizer;

    public GetAuditByIdQueryValidator(IStringLocalizer<GetAuditByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.audit.get.id.required"]);
    }
}

public class GetAuditByIdQueryHandler : IRequestHandler<GetAuditByIdQuery, Result<FullAuditDto>>
{
    private readonly IAuditLogDbContext _context;
    private readonly IStringLocalizer<GetAuditByIdQueryHandler> _localizer;

    public GetAuditByIdQueryHandler(
        IAuditLogDbContext context,
        IStringLocalizer<GetAuditByIdQueryHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<FullAuditDto>> Handle(GetAuditByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Audits
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var audit = new FullAuditDto
        {
            Id = entity.Id,
            IpAddress = entity.IpAddress,
            UserId = entity.UserId,
            Email = entity.Email,
            FullName = entity.FullName,
            AuditData = entity.AuditData,
            EntityType = entity.EntityType,
            DateTime = entity.AuditDateTimeUtc,
            Action = AuditAction.From(entity.Action),
            TablePk = entity.TablePk
        };

        return Result<FullAuditDto>.Success(audit);
    }
}
