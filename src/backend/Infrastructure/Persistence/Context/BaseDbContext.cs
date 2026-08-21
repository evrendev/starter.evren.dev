using System.Data;
using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Identity;
using EvrenDev.Infrastructure.Auditing;
using EvrenDev.Infrastructure.Identity;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Persistence.Context;

public abstract class BaseDbContext(
        IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
        DbContextOptions options,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
    // 9th type param (TUserPasskey) is new in .NET 10 Identity (WebAuthn/passkey support) —
    // unrelated to Finbuckle's own v10 changes, just a coincidental version alignment.
    : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>,
        IdentityUserPasskey<string>>(multiTenantContextAccessor, options)
{
    private readonly DatabaseSettings _dbSettings = dbSettings.Value;
    protected readonly ICurrentUser CurrentUser = currentUser;

    // Used by Dapper
    public IDbConnection Connection => Database.GetDbConnection();

    public DbSet<Trail> AuditTrails => Set<Trail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QueryFilters need to be applied before base.OnModelCreating
        modelBuilder.AppendGlobalQueryFilter<ISoftDelete>("SoftDelete", s => s.DeletedOn == null);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO: We want this only for development probably... maybe better make it configurable in logger.json config?
        optionsBuilder.EnableSensitiveDataLogging();

        // If you want to see the sql queries that efcore executes:

        // Uncomment the next line to see them in the output window of visual studio
        // optionsBuilder.LogTo(m => Debug.WriteLine(m), LogLevel.Information);

        // Or uncomment the next line if you want to see them in the console
        // optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

        // Base class's own TenantInfo property is now ITenantInfo-typed (ConnectionString
        // isn't on that interface as of v7+) — read through the strongly-typed accessor
        // instead to reach our concrete TenantInfo's ConnectionString property.
        var connectionString = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.ConnectionString;
        if (!string.IsNullOrWhiteSpace(connectionString))
            optionsBuilder.UseDatabase(_dbSettings.DbProvider!, connectionString);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var auditEntries = HandleAuditingBeforeSaveChanges(CurrentUser.GetUserId());

        // Runs after parents' DeletedOn is already set above, so it can see which
        // entities were *just* soft-deleted this call and cascade into their
        // DeleteBehavior.Cascade children (Task V1) — deliberately excludes cascade
        // audit-trail entries for the children it touches; only the top-level,
        // explicitly-deleted entity gets a Delete AuditTrail row (see HandleAuditingBeforeSaveChanges above)
        await CascadeSoftDeleteAsync(CurrentUser.GetUserId(), cancellationToken);

        var result = await base.SaveChangesAsync(cancellationToken);

        await HandleAuditingAfterSaveChangesAsync(auditEntries, cancellationToken);

        await SendDomainEventsAsync();

        return result;
    }

    private List<AuditTrail> HandleAuditingBeforeSaveChanges(Guid userId)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.LastModifiedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = userId;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = userId;
                        softDelete.DeletedOn = DateTime.UtcNow;
                        entry.State = EntityState.Modified;
                    }

                    break;
            }
        }

        ChangeTracker.DetectChanges();

        var trailEntries = new List<AuditTrail>();
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>()
                     .Where(e => e.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
                     .ToList())
        {
            var trailEntry = new AuditTrail(entry, serializer)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            trailEntries.Add(trailEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    trailEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    trailEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trailEntry.TrailType = TrailType.Create;
                        trailEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trailEntry.TrailType = TrailType.Delete;
                        trailEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (entry.Entity is ISoftDelete && IsSoftDeleteTransition(property))
                        {
                            trailEntry.ChangedColumns.Add(propertyName);
                            trailEntry.TrailType = TrailType.Delete;
                            trailEntry.OldValues[propertyName] = property.OriginalValue;
                            trailEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        else if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            trailEntry.ChangedColumns.Add(propertyName);
                            trailEntry.TrailType = TrailType.Update;
                            trailEntry.OldValues[propertyName] = property.OriginalValue;
                            trailEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                }
            }
        }

        foreach (var auditEntry in trailEntries.Where(e => !e.HasTemporaryProperties))
            AuditTrails.Add(auditEntry.ToAuditTrail());

        return trailEntries.Where(e => e.HasTemporaryProperties).ToList();
    }

    private Task HandleAuditingAfterSaveChangesAsync(List<AuditTrail> trailEntries,
        CancellationToken cancellationToken = new())
    {
        if (trailEntries is null || trailEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var entry in trailEntries)
        {
            foreach (var prop in entry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                else
                    entry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            AuditTrails.Add(entry.ToAuditTrail());
        }

        return SaveChangesAsync(cancellationToken);
    }

    // Shared by both the audit-trail classification above (Delete vs Update) and
    // CascadeSoftDeleteAsync below — true only when DeletedOn just moved from null
    // to non-null in this SaveChanges call, never for an ordinary property update
    // that happens to leave DeletedOn untouched.
    private static bool IsSoftDeleteTransition(PropertyEntry property) =>
        property.Metadata.Name == nameof(ISoftDelete.DeletedOn)
        && property.IsModified
        && property.OriginalValue is null
        && property.CurrentValue is not null;

    // Task V1: when an entity is soft-deleted, walk every DeleteBehavior.Cascade
    // collection navigation (via EF Core's own model metadata — no per-entity code,
    // no reflection) and soft-delete its children too, recursively. Runs in a loop
    // because soft-deleting a child adds it to the ChangeTracker, which may itself
    // have its own cascade children still to discover — a single top-to-bottom pass
    // would miss anything below the second level (see Task V0 finding (d)).
    // DeleteBehavior.Restrict relationships (PaymentOrder->Course, Task Q1) are
    // never touched: their DeleteBehavior isn't Cascade, so the filter below skips them.
    private async Task CascadeSoftDeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var processed = new HashSet<object>();
        bool changed;

        do
        {
            changed = false;
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries<ISoftDelete>().ToList())
            {
                if (!processed.Add(entry.Entity))
                    continue;

                if (!IsSoftDeleteTransition(entry.Property(nameof(ISoftDelete.DeletedOn))))
                    continue;

                var entityType = Model.FindEntityType(entry.Entity.GetType());
                if (entityType is null)
                    continue;

                foreach (var navigation in entityType.GetNavigations())
                {
                    if (!navigation.IsCollection)
                        continue;

                    var foreignKey = navigation.ForeignKey;
                    if (foreignKey.DeleteBehavior != DeleteBehavior.Cascade)
                        continue;

                    // Only walk parent->child; DependentToPrincipal points back up
                    if (foreignKey.PrincipalToDependent != navigation)
                        continue;

                    await Entry(entry.Entity).Collection(navigation.Name).LoadAsync(cancellationToken);

                    var children = (System.Collections.IEnumerable)navigation.GetGetter().GetClrValue(entry.Entity)!;
                    foreach (var child in children)
                    {
                        // A child that doesn't implement ISoftDelete can't be cascaded
                        // into by this mechanism — skip it silently rather than throw;
                        // its own DB-level ON DELETE CASCADE still applies on a real hard delete.
                        if (child is not ISoftDelete softDeleteChild || softDeleteChild.DeletedOn is not null)
                            continue;

                        softDeleteChild.DeletedOn = DateTime.UtcNow;
                        softDeleteChild.DeletedBy = userId;
                        Entry(child).State = EntityState.Modified;
                        changed = true;
                    }
                }
            }
        } while (changed);
    }

    private async Task SendDomainEventsAsync()
    {
        var entitiesWithEvents = ChangeTracker.Entries<IEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var domainEvents = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();
            foreach (var domainEvent in domainEvents)
                await events.PublishAsync(domainEvent);
        }
    }
}
