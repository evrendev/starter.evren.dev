using EvrenDev.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Catalog.Configurations;

public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(e => e.Colour, cb =>
            cb.Property(e => e.Code)
                .HasMaxLength(7)
                .HasDefaultValue(Colour.White.Code)
                .HasColumnName("Colour")
                .IsRequired(false)
        );
    }
}
