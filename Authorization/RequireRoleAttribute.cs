using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }

    public static int GetRoleLevel(string roleName)
    {
        return roleName.ToLowerInvariant() switch
        {
            "admin" => 2,
            "moderator" => 1,
            "user" => 0,
            _ => -1
        };
    }

    public static bool HasMinimumRole(ClaimsPrincipal user, string minimumRole)
    {
        var userRoleStr = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(userRoleStr)) return false;

        if (!Enum.TryParse<UserRole>(userRoleStr, true, out var userRole)) return false;

        var minimum = GetRoleLevel(minimumRole);
        if (minimum < 0) return false;

        return (int)userRole >= minimum;
    }

    public static UserRole? GetCurrentRole(ClaimsPrincipal user)
    {
        var roleStr = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleStr)) return null;

        return Enum.TryParse<UserRole>(roleStr, true, out var role) ? role : null;
    }
}
