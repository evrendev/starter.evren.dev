using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class QuestionConfig : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Prompt)
            .IsRequired();

        builder.Property(b => b.Order)
            .IsRequired();

        builder.HasOne(b => b.Page)
            .WithMany(p => p.Questions)
            .HasForeignKey(b => b.PageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
