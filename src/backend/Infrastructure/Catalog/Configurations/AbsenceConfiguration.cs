using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Catalog.Configurations;

public class AbsenceConfiguration : IEntityTypeConfiguration<Absence>
{
    public void Configure(EntityTypeBuilder<Absence> builder)
    {
        builder.Property(t => t.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(t => t.Location)
            .HasMaxLength(50)
            .IsRequired(true);

        builder.Property(t => t.Employee)
            .HasMaxLength(100)
            .IsRequired(true);

        builder.Property(t => t.CalendarId)
            .HasMaxLength(10)
            .IsRequired(true);
    }
}
