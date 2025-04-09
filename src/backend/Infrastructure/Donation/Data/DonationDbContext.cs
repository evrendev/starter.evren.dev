using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Donation;

namespace EvrenDev.Infrastructure.Donation.Data;
public class DonationDbContext : DbContext, IDonationDbContext
{
    public DonationDbContext(DbContextOptions<DonationDbContext> options)
        : base(options) { }

    public DbSet<FontainDonation> FontainDonations => Set<FontainDonation>();
}


