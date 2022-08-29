using Api.Constants;
using System.Security.Claims;

namespace Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static List<Claim>? GetClaims(this HttpContext context)
        {
            return (context.User.Identity as ClaimsIdentity)
                ?.Claims
                ?.ToList();
        }

        public static Guid GetCurrentUserId(this HttpContext context)
        {
            var userId = context.GetClaims()
                ?.FirstOrDefault(p => p.Type == "id")?.Value;

            if(!Guid.TryParse(userId, out Guid result))
            {
                throw new UnauthorizedAccessException();
            }

            return result;
        }

        public static bool UserHasRole(this List<Claim>? usersClaims, string[] requiredRoles)
        {
            var hasRole = usersClaims?.Any(p => p.Type == ClaimTypes.Role
                && (p.Value == KnownRoles.ADMINISTRATOR || requiredRoles.Contains(p.Value)));

            return hasRole == null
                ? false
                : hasRole.Value;
        }
    }
}
