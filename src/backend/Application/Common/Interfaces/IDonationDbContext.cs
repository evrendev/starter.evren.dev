using EvrenDev.Domain.Entities.Donation;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Common.Interfaces;

public interface IDonationDbContext
{
    DbSet<FountainDonation> FountainDonations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
