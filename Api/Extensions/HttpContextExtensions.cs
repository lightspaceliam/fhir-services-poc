using Api.Constants;
using Api.Models;
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

        public static CurrentUserCredential ToCurrentUserCredentials(this HttpContext context)
        {
            //  TODO: currently mocked to an existing Fhir healthcare provider. UserId is not currently supported, therefore is currently mocked.
            //  Newly updated since Fhir service update for Tenant1
            return new CurrentUserCredential
            {
                HealthCareProviderId = "d4d44f0d-2a7c-4dcc-b06e-5be4bf1b8bc4",
                UserId = "Todo-12345",
                TenantIdentifier = "Tenant1"
            };
        }
    }
}
