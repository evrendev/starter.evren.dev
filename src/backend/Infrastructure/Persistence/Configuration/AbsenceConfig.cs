using EvrenDev.Domain.Catalog;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class AbsenceConfig : IEntityTypeConfiguration<Absence>
{
    public void Configure(EntityTypeBuilder<Absence> builder)
    {
        builder.IsMultiTenant();

        builder.Property(t => t.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(t => t.Location)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Employee)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.CalendarId)
            .HasMaxLength(100)
            .IsRequired();
    }
}
