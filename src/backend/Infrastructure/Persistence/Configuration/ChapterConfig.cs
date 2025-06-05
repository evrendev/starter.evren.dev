using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class ChapterConfig : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Title)
            .HasMaxLength(256)
            .IsRequired(true);

        builder.HasMany(b => b.Lessons)
            .WithOne(b => b.Chapter)
            .HasForeignKey(b => b.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
