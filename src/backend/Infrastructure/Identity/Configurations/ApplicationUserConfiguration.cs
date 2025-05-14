using EvrenDev.Domain.Entities.Identity;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Identity.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Image)
            .HasMaxLength(100)
            .HasDefaultValue("default.jpg")
            .IsRequired(false);

        builder.OwnsOne(e => e.Language, gb =>
            gb.Property(e => e.Code)
                .HasMaxLength(2)
                .HasColumnName("Language")
        );

        builder.OwnsOne(e => e.Gender, gb =>
            gb.Property(e => e.Code)
                .HasMaxLength(4)
                .HasColumnName("Gender")
        );

        builder.IsMultiTenant();
        builder.ToTable("Users");
    }
}
