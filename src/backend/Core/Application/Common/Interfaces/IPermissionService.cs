namespace EvrenDev.Application.Common.Interfaces;
public interface IPermissionService
{
    Task<List<string>> GetUserPermissions(Guid userId);
    Task<bool> HasPermission(Guid userId, string permission);
    Task<bool> HasAnyPermission(Guid userId, IEnumerable<string> permissions);
    Task<bool> HasAllPermissions(Guid userId, IEnumerable<string> permissions);
}
