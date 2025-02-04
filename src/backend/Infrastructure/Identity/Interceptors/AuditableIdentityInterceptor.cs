using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EvrenDev.Infrastructure.Identity.Interceptors;

public class AuditableIdentityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _user;
    private readonly TimeProvider _dateTime;

    public AuditableIdentityInterceptor(
        ICurrentUser user,
        TimeProvider dateTime)
    {
        _user = user;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<IBaseIdentityEntity>();

        foreach (var entry in entries)
        {
            var utcNow = _dateTime.GetUtcNow();

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedTime = utcNow;
                    entry.Entity.Creator = _user.Email;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedTime = utcNow;
                    entry.Entity.Modifier = _user.Email;
                    break;

                case EntityState.Deleted:
                    entry.Entity.Delete(_user.Email);

                    entry.State = EntityState.Modified;
                    break;

                default:
                    entry.Entity.Modifier = _user.Email;
                    entry.Entity.ModifiedTime = utcNow;
                    continue;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
