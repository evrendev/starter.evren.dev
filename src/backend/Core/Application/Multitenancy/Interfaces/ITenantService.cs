using EvrenDev.Application.Multitenancy.Entities;
using EvrenDev.Application.Multitenancy.Commands.Create;
using EvrenDev.Application.Multitenancy.Commands.Update;

namespace EvrenDev.Application.Multitenancy.Interfaces;

public interface ITenantService
{
    Task<List<TenantDto>> GetAllAsync();
    Task<bool> ExistsWithIdAsync(string id);
    Task<bool> ExistsWithNameAsync(string name);
    Task<TenantDto> GetByIdAsync(string id);
    Task<string> CreateAsync(CreateTenantCommand command, CancellationToken cancellationToken);
    Task<string> UpdateAsync(UpdateTenantCommand command, CancellationToken cancellationToken);
    Task<string> ActivateAsync(string id);
    Task<string> DeactivateAsync(string id);
    Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate);
}
