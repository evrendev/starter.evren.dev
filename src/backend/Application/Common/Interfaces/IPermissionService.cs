namespace EvrenDev.Application.Common.Interfaces;
public interface IPermissionService
{
    Task<List<string>> GetUserPermissions(string userId);
    Task<List<string>> GetRolePermissions(string roleId);
    Task<bool> HasPermission(string userId, string permission);
    Task<bool> HasAnyPermission(string userId, IEnumerable<string> permissions);
    Task<bool> HasAllPermissions(string userId, IEnumerable<string> permissions);
    Task UpdateRolePermissions(string roleId, IEnumerable<string> permissions);
}
