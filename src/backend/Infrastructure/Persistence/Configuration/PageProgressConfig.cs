using EvrenDev.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class PageProgressConfig : IEntityTypeConfiguration<PageProgress>
{
    public void Configure(EntityTypeBuilder<PageProgress> builder)
    {
        builder.HasKey(e => new { e.UserId, e.PageId });

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        builder.HasOne(e => e.Page)
            .WithMany(p => p.Progress)
            .HasForeignKey(e => e.PageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
