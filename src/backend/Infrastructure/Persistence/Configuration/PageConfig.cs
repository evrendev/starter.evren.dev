using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class PageConfig : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
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

        builder.HasOne(b => b.Chapter)
            .WithMany(c => c.Pages)
            .HasForeignKey(b => b.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
