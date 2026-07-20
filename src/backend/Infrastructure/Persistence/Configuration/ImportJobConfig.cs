using EvrenDev.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class ImportJobConfig : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        // No IsMultiTenant(): same rationale as CourseEnrollment/ChapterProgress/PageProgress
        // (docs/lms-domain.md "Multi-Tenancy") — reached only via CourseId FK, already isolated by
        // the tenant's own database.
        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.TotalSlides)
            .IsRequired();

        builder.Property(p => p.ProcessedSlides)
            .IsRequired();

        builder.Property(p => p.SucceededSlides)
            .IsRequired();

        builder.Property(p => p.FailedSlides)
            .IsRequired();
    }
}
