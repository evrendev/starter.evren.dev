using EvrenDev.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class ChapterProgressConfig : IEntityTypeConfiguration<ChapterProgress>
{
    public void Configure(EntityTypeBuilder<ChapterProgress> builder)
    {
        builder.HasKey(e => new { e.UserId, e.ChapterId });

        builder.HasOne(e => e.User)
            .WithMany(u => u.Progress)
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.Chapter)
            .WithMany(c => c.Progress)
            .HasForeignKey(e => e.ChapterId);
    }
}
