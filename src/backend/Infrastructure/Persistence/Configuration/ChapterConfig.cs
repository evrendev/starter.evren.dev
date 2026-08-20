using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class ChapterConfig : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(b => b.Order)
            .IsRequired();

        builder.Property(b => b.IsStaging)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(b => b.Pages)
            .WithOne(b => b.Chapter)
            .HasForeignKey(b => b.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
