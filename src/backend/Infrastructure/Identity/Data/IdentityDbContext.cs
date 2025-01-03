using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EvrenDev.Infrastructure.Identity.Data;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Image)
                .HasMaxLength(100)
                .HasDefaultValue("default.jpg")
                .IsRequired(false);

            entity.OwnsOne(e => e.Language, gb =>
                gb.Property(e => e.Code)
                    .HasMaxLength(2)
                    .HasColumnName("Language")
            );

            entity.OwnsOne(e => e.Gender, gb =>
                gb.Property(e => e.Code)
                    .HasMaxLength(4)
                    .HasColumnName("Gender")
            );

            entity.ToTable("Users");
        });

        builder.Entity<IdentityRole>(entity => entity.ToTable("Roles"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));

        builder.HasDefaultSchema("Identity");
    }
}
