using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class NoteConfig : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(b => b.Content)
            .HasMaxLength(256);

        builder.HasOne(b => b.Lesson)
            .WithMany(l => l.Notes)
            .HasForeignKey(b => b.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Notes)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
