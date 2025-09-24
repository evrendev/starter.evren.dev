using EvrenDev.Application.Common.Events;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Catalog;
using EvrenDev.Infrastructure.Persistence.Configuration;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Options;

namespace EvrenDev.Infrastructure.Persistence.Context;

public class ApplicationDbContext(
        ITenantInfo currentTenant,
        DbContextOptions options,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
    : BaseDbContext(currentTenant, options, currentUser, serializer, dbSettings, events)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}
