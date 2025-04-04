using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Common.Interfaces;

public interface IDonationDbContext
{
    DbSet<BrunnenDonation> BrunnenDonations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
