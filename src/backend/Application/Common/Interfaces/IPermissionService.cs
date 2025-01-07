namespace EvrenDev.Application.Common.Interfaces;
public interface IPermissionService
{
    Task<List<string>> GetUserPermissions(string userId);
    Task<bool> HasPermission(string userId, string permission);
    Task<bool> HasAnyPermission(string userId, IEnumerable<string> permissions);
    Task<bool> HasAllPermissions(string userId, IEnumerable<string> permissions);
}
