using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class CourseConfig : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Title)
            .HasMaxLength(256)
            .IsRequired(true);

        builder.Property(b => b.Image)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasMany(b => b.Chapters)
            .WithOne(b => b.Course)
            .HasForeignKey(b => b.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
