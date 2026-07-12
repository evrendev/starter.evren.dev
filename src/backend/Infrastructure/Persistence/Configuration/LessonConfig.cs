using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class LessonConfig : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasMany(b => b.Pages)
            .WithOne(p => p.Lesson)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
