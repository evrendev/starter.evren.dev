using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class OptionConfig : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Label)
            .IsRequired();

        builder.Property(b => b.IsCorrect)
            .IsRequired();

        builder.Property(b => b.Order)
            .IsRequired();

        builder.HasOne(b => b.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(b => b.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
