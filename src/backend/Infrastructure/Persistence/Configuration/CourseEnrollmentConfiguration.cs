using EvrenDev.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class CourseEnrollmentConfig : IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.HasKey(e => new { e.UserId, e.CourseId });

        builder.HasOne(e => e.User)
            .WithMany(u => u.CourseEnrollments)
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.CourseEnrollments)
            .HasForeignKey(e => e.CourseId);
    }
}
