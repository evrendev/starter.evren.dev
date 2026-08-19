using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Identity.Students.Entities;
using EvrenDev.Application.Identity.Students.Interfaces;
using EvrenDev.Application.Identity.Students.Queries.Paginate;
using EvrenDev.Domain.Identity;
using EvrenDev.Domain.Payments;
using EvrenDev.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Infrastructure.Identity;

// Students combine data from three different persistence strategies (Identity
// via UserManager, Catalog/Payments aggregate roots via ApplicationDbContext
// directly) that no IRepository<T> spans — this mirrors UserService's own
// precedent of an Infrastructure-side service injecting ApplicationDbContext
// for Identity-adjacent reads, rather than adding cross-schema aggregate
// support to IReadRepository<T> (see Task R0/R1 design discussion).
internal partial class StudentService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db)
    : IStudentService
{
    public async Task<PaginationResponse<StudentSummaryDto>> PaginatedListAsync(GetStudentsRequest filter,
        CancellationToken cancellationToken)
    {
        var studentUserIds = await GetStudentUserIdsQueryAsync(cancellationToken);

        var baseQuery = userManager.Users.Where(u => studentUserIds.Contains(u.Id));

        if (filter.IsActive.HasValue)
            baseQuery = baseQuery.Where(u => u.IsActive == filter.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search;
            baseQuery = baseQuery.Where(u =>
                (u.FirstName != null && u.FirstName.Contains(search)) ||
                (u.LastName != null && u.LastName.Contains(search)) ||
                (u.Email != null && u.Email.Contains(search)));
        }

        var count = await baseQuery.CountAsync(cancellationToken);

        var page = filter.Page <= 0 ? 1 : filter.Page;
        var itemsPerPage = filter.ItemsPerPage <= 0 ? int.MaxValue : filter.ItemsPerPage;

        var pagedUsers = await baseQuery
            .OrderBy(u => u.Email)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsActive, u.EmailConfirmed })
            .ToListAsync(cancellationToken);

        var userIds = pagedUsers.Select(u => u.Id).ToList();

        // Three GroupBy aggregates instead of per-user queries — avoids N+1
        // regardless of how many students are on the current page.
        var enrollmentCounts = await db.CourseEnrollments
            .Where(e => userIds.Contains(e.UserId))
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalPaidByUser = await db.PaymentOrders
            .Where(p => userIds.Contains(p.UserId) && p.Status == PaymentOrderStatus.Captured)
            .GroupBy(p => p.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync(cancellationToken);

        // ChapterProgress has no direct Course FK (only Chapter) — this
        // averages a student's percent-complete across every chapter they've
        // touched in any course, which is what "average completion" means at
        // student-summary granularity (see Task R0 finding: CourseEnrollment.
        // PercentComplete is never rolled up by any handler, so it can't be
        // used here).
        var avgCompletionByUser = await db.ChapterProgresses
            .Where(cp => userIds.Contains(cp.UserId))
            .GroupBy(cp => cp.UserId)
            .Select(g => new { UserId = g.Key, Avg = g.Average(cp => (double)cp.PercentComplete) })
            .ToListAsync(cancellationToken);

        var items = pagedUsers.Select(u => new StudentSummaryDto
        {
            UserId = u.Id,
            FullName = $"{u.FirstName} {u.LastName}".Trim(),
            Email = u.Email,
            IsActive = u.IsActive,
            EmailConfirmed = u.EmailConfirmed,
            EnrolledCourseCount = enrollmentCounts.FirstOrDefault(e => e.UserId == u.Id)?.Count ?? 0,
            TotalPaid = totalPaidByUser.FirstOrDefault(p => p.UserId == u.Id)?.Total ?? 0m,
            AverageCompletionPercent = avgCompletionByUser.FirstOrDefault(a => a.UserId == u.Id)?.Avg ?? 0
        }).ToList();

        return new PaginationResponse<StudentSummaryDto>(items, count, page, itemsPerPage);
    }

    public async Task<StudentsSummaryStatsDto> GetSummaryStatsAsync(CancellationToken cancellationToken)
    {
        var studentUserIds = await GetStudentUserIdsQueryAsync(cancellationToken);

        var totalRevenue = await db.PaymentOrders
            .Where(p => studentUserIds.Contains(p.UserId) && p.Status == PaymentOrderStatus.Captured)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

        var avgCompletion = await db.ChapterProgresses
            .Where(cp => studentUserIds.Contains(cp.UserId))
            .Select(cp => (double?)cp.PercentComplete)
            .AverageAsync(cancellationToken) ?? 0;

        return new StudentsSummaryStatsDto
        {
            TotalRevenue = totalRevenue,
            AverageCompletionPercent = avgCompletion
        };
    }

    public async Task<StudentDetailDto> GetDetailAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsActive, u.EmailConfirmed })
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException($"Student with ID '{userId}' not found.");

        var enrollments = await db.CourseEnrollments
            .Where(e => e.UserId == userId)
            .Select(e => new StudentEnrollmentDto
            {
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                EnrolledAt = e.EnrolledAt,
                PricePaid = e.PricePaid,
                PercentComplete = e.PercentComplete
            })
            .ToListAsync(cancellationToken);

        // Not ordered by CreatedOn: AuditableEntity.CreatedOn has no public
        // setter, so EF Core can't translate it in a query (get-only
        // auto-property, not a mapped column for ordering purposes) — order
        // by CapturedAt instead, which is a real nullable column.
        var payments = await db.PaymentOrders
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CapturedAt)
            .Select(p => new StudentPaymentDto
            {
                CourseId = p.CourseId,
                CourseTitle = p.Course.Title,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status.ToString(),
                PayPalCaptureId = p.PayPalCaptureId,
                CapturedAt = p.CapturedAt
            })
            .ToListAsync(cancellationToken);

        return new StudentDetailDto
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Email = user.Email,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            Enrollments = enrollments,
            Payments = payments
        };
    }

    private async Task<List<string>> GetStudentUserIdsQueryAsync(CancellationToken cancellationToken)
    {
        var studentRoleId = await roleManager.Roles
            .Where(r => r.Name == ApiRoles.Student)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return await db.UserRoles
            .Where(ur => ur.RoleId == studentRoleId)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);
    }
}
