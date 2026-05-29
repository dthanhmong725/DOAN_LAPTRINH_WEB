using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Authorization;

public static class AuthorizationPolicies
{
    public const string RequireModerator = "RequireModerator";
    public const string RequireAdmin = "RequireAdmin";

    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(RequireModerator, policy =>
            policy.RequireAssertion(context =>
                HasModeratorOrHigherRole(context.User)));

        options.AddPolicy(RequireAdmin, policy =>
            policy.RequireAssertion(context =>
                HasAdminRole(context.User)));
    }

    private static bool HasModeratorOrHigherRole(ClaimsPrincipal user)
    {
        var roleStr = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleStr)) return false;

        return Enum.TryParse<UserRole>(roleStr, out var role) &&
               (role == UserRole.Moderator || role == UserRole.Admin);
    }

    private static bool HasAdminRole(ClaimsPrincipal user)
    {
        var roleStr = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleStr)) return false;

        return Enum.TryParse<UserRole>(roleStr, out var role) &&
               role == UserRole.Admin;
    }
}
