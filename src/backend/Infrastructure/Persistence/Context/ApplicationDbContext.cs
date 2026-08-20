using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Payments;
using EvrenDev.Infrastructure.Persistence.Configuration;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Options;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Persistence.Context;

public class ApplicationDbContext(
        IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
        DbContextOptions options,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
    : BaseDbContext(multiTenantContextAccessor, options, currentUser, serializer, dbSettings, events)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<ChapterProgress> ChapterProgresses => Set<ChapterProgress>();
    public DbSet<PageProgress> PageProgresses => Set<PageProgress>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
    public DbSet<PaymentOrder> PaymentOrders => Set<PaymentOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}
