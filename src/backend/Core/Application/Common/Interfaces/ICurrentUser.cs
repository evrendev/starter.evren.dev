namespace EvrenDev.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Id { get; }
    string? Email { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
}
