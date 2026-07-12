using EvrenDev.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class LessonPageProgressConfig : IEntityTypeConfiguration<LessonPageProgress>
{
    public void Configure(EntityTypeBuilder<LessonPageProgress> builder)
    {
        builder.HasKey(e => new { e.UserId, e.LessonPageId });

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.LessonPage)
            .WithMany(p => p.Progress)
            .HasForeignKey(e => e.LessonPageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
