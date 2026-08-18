using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Identity.Students.Entities;
using EvrenDev.Application.Identity.Students.Queries.Paginate;

namespace EvrenDev.Application.Identity.Students.Interfaces;

public interface IStudentService : ITransientService
{
    Task<PaginationResponse<StudentSummaryDto>> PaginatedListAsync(GetStudentsRequest filter,
        CancellationToken cancellationToken);

    Task<StudentsSummaryStatsDto> GetSummaryStatsAsync(CancellationToken cancellationToken);
}
