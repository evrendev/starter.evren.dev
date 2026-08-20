using EvrenDev.Domain.Payments;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvrenDev.Infrastructure.Persistence.Configuration;

public class PaymentOrderConfig : IEntityTypeConfiguration<PaymentOrder>
{
    public void Configure(EntityTypeBuilder<PaymentOrder> builder)
    {
        builder.ToTable("PaymentOrders", SchemaNames.Payments);

        // Task Q1 explicit instruction — note this is a deliberate exception to
        // CLAUDE.md's "IsMultiTenant() only on the Category→Course→Chapter→
        // Lesson→LessonPage chain" rule: PaymentOrder isn't in that navigation
        // chain, but still needs tenant isolation since it references Course.
        builder.IsMultiTenant();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.PayPalOrderId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(p => p.PayPalCaptureId)
            .HasMaxLength(64);

        // Deliberately NOT cascade: a PaymentOrder is a financial record and
        // must survive even if its Course is later deleted (soft-delete makes
        // this mostly moot day-to-day, but a hard delete must never take
        // payment history down with it — see Task Q1 report).
        builder.HasOne(p => p.Course)
            .WithMany()
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
