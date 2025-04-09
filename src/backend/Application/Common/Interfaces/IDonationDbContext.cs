using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Common.Interfaces;

public interface IDonationDbContext
{
    DbSet<FontainDonation> FontainDonations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
