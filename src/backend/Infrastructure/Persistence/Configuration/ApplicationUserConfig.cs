using EvrenDev.Domain.Common.Events.Identity;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .ToTable("Users", SchemaNames.Identity)
            .IsMultiTenant();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired(true);

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired(true);

        builder.Property(u => u.PlaceOfBirth)
            .HasMaxLength(100);

        builder
            .Property(u => u.ObjectId)
                .HasMaxLength(256);
    }
}
