namespace EvrenDev.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    string? TenantId { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? FullName { get; }
}
