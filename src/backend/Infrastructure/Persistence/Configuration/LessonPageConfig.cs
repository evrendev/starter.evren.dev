using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class LessonPageConfig : IEntityTypeConfiguration<LessonPage>
{
    public void Configure(EntityTypeBuilder<LessonPage> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(b => b.Content)
            .IsRequired();

        builder.Property(b => b.ContentType)
            .IsRequired();

        builder.Property(b => b.Order)
            .IsRequired();

        builder.Property(b => b.NeedsReview)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.IsImported)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(b => b.Lesson)
            .WithMany(l => l.Pages)
            .HasForeignKey(b => b.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
