using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using College.BLL.Common.Extensions;

namespace College.BLL.Common;

public static class GettingUserProperties
{
    public static string GetUserId(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        string userId = GetUserId(httpContext.User) ?? string.Empty;

        if (string.IsNullOrEmpty(userId))
        {
            ThrowAuthenticationException(nameof(IdentityResourceClaimsTypes.Uid));
        }

        return userId;
    }

    public static string GetUserId(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return user.GetUserPropertyByClaimType(IdentityResourceClaimsTypes.Uid);
    }

    private static void ThrowAuthenticationException(string claimType)
        => throw new AuthenticationException($"Can not get user's claim {claimType} from Context.");
}
