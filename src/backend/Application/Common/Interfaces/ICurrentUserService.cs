namespace EvrenDev.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? Id { get; }
    string? Email { get; }
    string? FullName { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}
