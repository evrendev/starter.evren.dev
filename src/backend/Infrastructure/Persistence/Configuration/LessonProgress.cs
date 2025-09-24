using EvrenDev.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class LessonProgressConfig : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.HasKey(e => new { e.UserId, e.LessonId });

        builder.HasOne(e => e.User)
            .WithMany(u => u.Progress)
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.Lesson)
            .WithMany(c => c.Progress)
            .HasForeignKey(e => e.LessonId);
    }
}
